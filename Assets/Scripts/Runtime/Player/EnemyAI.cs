using UnityEngine;

/// <summary>
/// Drives enemy tank AI using a pure Rigidbody2D steering system.
/// Replaces the NavMeshAgent approach so no NavMesh baking is required.
///
/// Flanker  – sneaks to the rear of the player's tank and shoots from behind.
/// Aggro    – interposes between the player and any active Flankers to draw
///            fire; starts with doubled health and 30% faster reload.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    // ────────────────────────────────────────────────────────────────────────
    // Types
    // ────────────────────────────────────────────────────────────────────────

    public enum EnemyType { Aggro, Flanker }

    // ────────────────────────────────────────────────────────────────────────
    // Inspector Fields
    // ────────────────────────────────────────────────────────────────────────

    [Header("Enemy Type")]
    public EnemyType type = EnemyType.Aggro;

    [Header("Target")]
    public Transform target;

    [Header("Movement")]
    public float moveSpeed = 2.2f;
    /// <summary>Body rotation speed in degrees per second.</summary>
    public float rotationSpeed = 140f;
    /// <summary>Distance from the desired position at which the tank stops advancing.</summary>
    public float stoppingDistance = 7f;

    [Header("Shooting")]
    public float shootingRange = 15f;

    [Header("Detection")]
    [Tooltip("Player must be within this radius for the AI to leave its Idle state. " +
             "Beyond it the tank stands completely still.")]
    public float detectionRange = 20f;

    [Header("Obstacle Avoidance")]
    [Tooltip("Length of each whisker raycast.")]
    public float avoidanceLookAhead = 2.5f;
    [Tooltip("Half-angle (degrees) of the side whiskers relative to forward.")]
    public float whiskerAngle = 40f;
    [Tooltip("Layers treated as solid obstacles (e.g. Default for walls). " +
             "Leave at 0 to auto-detect: excludes Body and Turet layers.")]
    public LayerMask obstacleLayer;

    [Header("Flanker Settings")]
    [Tooltip("How far behind the player the Flanker aims to position itself.")]
    public float flankOffset = 7f;

    [Header("Aggro Settings")]
    [Tooltip("Fractional health bonus applied on top of base health. " +
             "1.0 = +100% (doubles health). Requires 3 player shots if player bullet damage is ~67.")]
    public float aggroHealthBonus = 1f;
    [Tooltip("Fire-rate time multiplier. 0.7 = 30% faster reload.")]
    public float aggroReloadMultiplier = 0.7f;
    [Tooltip("Minimum distance the Aggro tank maintains from its interpose target.")]
    public float aggroKeepDistance = 6f;

    // ────────────────────────────────────────────────────────────────────────
    // Private State
    // ────────────────────────────────────────────────────────────────────────

    private Rigidbody2D rb;
    private EnemyShooting shooting;
    private Collider2D[] ownColliders;

    // Flanker: drifts the lateral offset for variety among multiple Flankers.
    private float flankDriftAngle;

    // Aggro: cache flanker positions to avoid FindObjectsByType every frame.
    private float flankerCacheTimer;
    private const float FlankerCacheInterval = 1.5f;
    private Vector2 cachedFlankerMidpoint;
    private bool hasCachedFlankers;

    // Reusable non-alloc buffer for CircleCastNonAlloc.
    private static readonly RaycastHit2D[] HitBuffer = new RaycastHit2D[16];

    // ────────────────────────────────────────────────────────────────────────
    // Lifecycle
    // ────────────────────────────────────────────────────────────────────────

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Collect own colliders once so whisker casts can filter them out.
        ownColliders = GetComponentsInChildren<Collider2D>(true);

        // Fall back to a sensible obstacle mask if the field was left at 0.
        if (obstacleLayer == 0)
            obstacleLayer = ~LayerMask.GetMask("Body", "Turet");

        // Aggro stat modifiers are applied here in Awake so TankHealth.Start()
        // reads the updated maxHealth when it initialises currentHealth.
        if (type == EnemyType.Aggro)
            ApplyAggroStats();
    }

    void Start()
    {
        shooting = GetComponent<EnemyShooting>();
        flankDriftAngle = Random.Range(0f, 360f);

        if (target == null)
            AcquirePlayer();
    }

    /// <summary>
    /// Called automatically by Unity when this component is disabled.
    /// TankHealth.Die() disables this script, so this is the guaranteed
    /// hook to stop all movement the instant the enemy dies.
    /// </summary>
    void OnDisable()
    {
        if (rb == null) return;
        rb.linearVelocity = Vector2.zero;
        // Freeze all axes so physics can no longer slide or rotate the corpse.
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    void Update()
    {
        if (target == null) { AcquirePlayer(); return; }

        float distToPlayer = Vector2.Distance(transform.position, target.position);

        // ── Idle state ────────────────────────────────────────────────────────
        // If the player is outside the detection radius, stop completely and
        // skip all AI logic until the player re-enters.
        if (distToPlayer > detectionRange)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 desired = ComputeDesiredPosition();
        SteerTowards(desired);
        TryFireAtPlayer();
    }

    // ────────────────────────────────────────────────────────────────────────
    // Stat Modifiers (Aggro)
    // ────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Doubles the Aggro tank's health and reduces its reload time by 30%.
    /// Called in Awake so TankHealth.Start() picks up the updated maxHealth.
    /// </summary>
    private void ApplyAggroStats()
    {
        TankHealth health = GetComponent<TankHealth>();
        if (health != null)
            health.maxHealth = Mathf.RoundToInt(health.maxHealth * (1f + aggroHealthBonus));

        EnemyShooting es = GetComponent<EnemyShooting>();
        if (es != null)
            es.fireRate *= aggroReloadMultiplier;
    }

    // ────────────────────────────────────────────────────────────────────────
    // Player Acquisition
    // ────────────────────────────────────────────────────────────────────────

    private void AcquirePlayer()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) target = p.transform;
    }

    // ────────────────────────────────────────────────────────────────────────
    // Desired Position — per-type AI logic
    // ────────────────────────────────────────────────────────────────────────

    private Vector2 ComputeDesiredPosition()
    {
        return type == EnemyType.Flanker
            ? ComputeFlankerTarget()
            : ComputeAggroTarget();
    }

    /// <summary>
    /// Flanker target: the rear of the player's tank, offset by <see cref="flankOffset"/>
    /// along the player's -right axis (opposite its local forward in 2D).
    /// A slow sinusoidal lateral drift prevents multiple Flankers stacking.
    /// </summary>
    private Vector2 ComputeFlankerTarget()
    {
        flankDriftAngle = (flankDriftAngle + Time.deltaTime * 18f) % 360f;
        float lateral = Mathf.Sin(flankDriftAngle * Mathf.Deg2Rad) * (flankOffset * 0.3f);

        Vector2 rearOffset = -(Vector2)target.right * flankOffset;
        Vector2 sideOffset =  (Vector2)target.up    * lateral;

        return (Vector2)target.position + rearOffset + sideOffset;
    }

    /// <summary>
    /// Aggro target: a point interpolated 40% of the way between the player
    /// and the average position of all active Flanker tanks, acting as a
    /// physical shield. Falls back to the player position when no Flankers exist.
    /// </summary>
    private Vector2 ComputeAggroTarget()
    {
        flankerCacheTimer -= Time.deltaTime;
        if (flankerCacheTimer <= 0f)
        {
            flankerCacheTimer = FlankerCacheInterval;
            RefreshFlankerCache();
        }

        if (hasCachedFlankers)
            return Vector2.Lerp((Vector2)target.position, cachedFlankerMidpoint, 0.4f);

        return (Vector2)target.position;
    }

    private void RefreshFlankerCache()
    {
        EnemyAI[] all = FindObjectsByType<EnemyAI>(FindObjectsSortMode.None);
        Vector2 sum = Vector2.zero;
        int count = 0;

        foreach (EnemyAI e in all)
        {
            if (e != this && e.type == EnemyType.Flanker && e.isActiveAndEnabled)
            {
                sum += (Vector2)e.transform.position;
                count++;
            }
        }

        hasCachedFlankers = count > 0;
        cachedFlankerMidpoint = hasCachedFlankers ? sum / count : Vector2.zero;
    }

    // ────────────────────────────────────────────────────────────────────────
    // Steering & Movement
    // ────────────────────────────────────────────────────────────────────────

    private void SteerTowards(Vector2 desiredPos)
    {
        Vector2 toTarget = desiredPos - (Vector2)transform.position;
        float dist = toTarget.magnitude;

        if (dist < stoppingDistance)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 desiredDir = toTarget / dist; // pre-normalised
        Vector2 steerDir   = ComputeSteeringDirection(desiredDir);

        // Rotate the tank body toward the computed steering direction.
        float targetAngle  = Mathf.Atan2(steerDir.y, steerDir.x) * Mathf.Rad2Deg;
        float currentAngle = transform.eulerAngles.z;
        float newAngle     = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f, 0f, newAngle);

        // Throttle forward speed when the tank hasn't finished turning yet,
        // simulating realistic tracked-vehicle momentum.
        float angleDiff = Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle));
        float throttle  = Mathf.Clamp01(1f - angleDiff / 90f);
        rb.linearVelocity = (Vector2)transform.right * (moveSpeed * throttle);
    }

    /// <summary>
    /// Three-whisker context steering.
    /// Blends obstacle-avoidance normals with the desired direction so the tank
    /// smoothly veers around walls without losing track of its goal.
    /// </summary>
    private Vector2 ComputeSteeringDirection(Vector2 desiredDir)
    {
        bool centreBlocked = CastWhisker(desiredDir,  0f,           out Vector2 cn);
        bool leftBlocked   = CastWhisker(desiredDir,  whiskerAngle, out Vector2 ln);
        bool rightBlocked  = CastWhisker(desiredDir, -whiskerAngle, out Vector2 rn);

        if (!centreBlocked && !leftBlocked && !rightBlocked)
            return desiredDir; // Open path — no adjustment needed.

        Vector2 avoidance = Vector2.zero;
        if (centreBlocked) avoidance += cn;
        if (leftBlocked)   avoidance += ln;
        if (rightBlocked)  avoidance += rn;

        // Weight avoidance heavily so the tank actually turns, then blend back
        // toward the goal so it doesn't orbit obstacles indefinitely.
        Vector2 blended = desiredDir + avoidance * 2f;

        if (blended.sqrMagnitude < 0.001f)
            blended = RotateVec(desiredDir, 90f); // last resort: go perpendicular

        return blended.normalized;
    }

    /// <summary>
    /// Fires a single CircleCast whisker, ignoring own colliders.
    /// Returns true when an obstacle was hit; writes the surface normal to <paramref name="hitNormal"/>.
    /// </summary>
    private bool CastWhisker(Vector2 baseDir, float angleDeg, out Vector2 hitNormal)
    {
        Vector2 dir      = RotateVec(baseDir, angleDeg);
        int hitCount     = Physics2D.CircleCastNonAlloc(
            (Vector2)transform.position, 0.35f, dir, HitBuffer, avoidanceLookAhead, obstacleLayer);

        float nearest = float.MaxValue;
        hitNormal     = Vector2.zero;
        bool found    = false;

        for (int i = 0; i < hitCount; i++)
        {
            RaycastHit2D h = HitBuffer[i];
            if (IsOwnCollider(h.collider)) continue;
            if (h.distance < nearest)
            {
                nearest   = h.distance;
                hitNormal = h.normal;
                found     = true;
            }
        }
        return found;
    }

    private bool IsOwnCollider(Collider2D col)
    {
        foreach (Collider2D own in ownColliders)
            if (own == col) return true;
        return false;
    }

    // ────────────────────────────────────────────────────────────────────────
    // Shooting
    // ────────────────────────────────────────────────────────────────────────

    private void TryFireAtPlayer()
    {
        if (shooting == null) return;
        if (Vector2.Distance(transform.position, target.position) <= shootingRange)
            shooting.TryShoot();
    }

    // ────────────────────────────────────────────────────────────────────────
    // Utility
    // ────────────────────────────────────────────────────────────────────────

    private static Vector2 RotateVec(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float c = Mathf.Cos(rad), s = Mathf.Sin(rad);
        return new Vector2(v.x * c - v.y * s, v.x * s + v.y * c);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        // Detection range (white) — the outer "vision radius".
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Stopping distance (cyan) — inner comfort zone.
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);

        if (target == null) return;

        if (type == EnemyType.Flanker)
        {
            Vector2 rearPos = (Vector2)target.position - (Vector2)target.right * flankOffset;
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(rearPos, 0.5f);
            Gizmos.DrawLine(transform.position, rearPos);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, target.position);
        }
    }
#endif
}
