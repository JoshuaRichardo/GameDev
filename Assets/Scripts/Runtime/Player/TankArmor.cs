using UnityEngine;
using System.Collections;

public class TankArmor : MonoBehaviour
{
    public enum ArmorSide { Front, Left, Right, Rear }

    [Header("Damage as % of max health")]
    [Tooltip("Front hits deal a random percent between min and max.")]
    public float frontPercentMin = 3f;
    public float frontPercentMax = 5f;
    [Tooltip("Left/Right (side) hits.")]
    public float sidePercent = 30f;
    [Tooltip("Rear hits.")]
    public float rearPercent = 15f;

    [Header("Engine Hit (Rear) Immobilization")]
    [Tooltip("How long the tank cannot move after a rear/engine hit.")]
    public float stunDuration = 5f;

    [Header("System")]
    [Tooltip("When true, the 4 trigger parts are the authority for incoming damage; bullets ignore the main body collider.")]
    public bool useArmorParts = true;

    // --- State exposed to UI ---
    public int FrontHits { get; private set; }
    public int LeftHits { get; private set; }
    public int RightHits { get; private set; }
    public int RearHits { get; private set; }
    public bool IsStunned { get; private set; }
    public float StunTimeRemaining { get; private set; }

    /// <summary>Raised when a rear hit immobilizes the engine. Argument is the immobilize duration in seconds.</summary>
    public System.Action<float> OnEngineHit;

    private TankHealth health;
    private PlayerTank playerMovement;

    void Awake()
    {
        health = GetComponent<TankHealth>();
        playerMovement = GetComponent<PlayerTank>();
    }

    /// <summary>
    /// Called by a <see cref="TankArmorPart"/> when one of the 4 body trigger zones is hit by a bullet.
    /// </summary>
    public void RegisterHit(ArmorSide side, int bulletDamage)
    {
        int maxHp = health != null ? health.GetMaxHealth() : 100;

        switch (side)
        {
            case ArmorSide.Front:
                FrontHits++;
                ApplyPercent(maxHp, Random.Range(frontPercentMin, frontPercentMax));
                break;

            case ArmorSide.Left:
                LeftHits++;
                ApplyPercent(maxHp, sidePercent);
                break;

            case ArmorSide.Right:
                RightHits++;
                ApplyPercent(maxHp, sidePercent);
                break;

            case ArmorSide.Rear:
                RearHits++;
                ApplyPercent(maxHp, rearPercent);
                // Engine hit: notify and immobilize.
                if (!IsStunned)
                {
                    OnEngineHit?.Invoke(stunDuration);
                    StartCoroutine(StunRoutine());
                }
                break;
        }
    }

    private void ApplyPercent(int maxHp, float percent)
    {
        int dmg = Mathf.Max(1, Mathf.RoundToInt(maxHp * percent / 100f));
        if (health != null) health.TakeDamage(dmg);
    }

    private IEnumerator StunRoutine()
    {
        IsStunned = true;

        if (playerMovement == null) playerMovement = GetComponent<PlayerTank>();
        if (playerMovement != null) playerMovement.enabled = false;

        StunTimeRemaining = stunDuration;
        while (StunTimeRemaining > 0f)
        {
            StunTimeRemaining -= Time.deltaTime;
            yield return null;
        }
        StunTimeRemaining = 0f;

        if (playerMovement != null) playerMovement.enabled = true;
        IsStunned = false;
    }
}
