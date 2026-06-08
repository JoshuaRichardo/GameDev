using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyType { Agro, Flanking }
    public EnemyType type = EnemyType.Agro;

    public Transform target;
    public float stoppingDistance = 10f;
    public float moveSpeed = 1.1f; 
    public float shootingRange = 15f;
    
    [Header("Flanking Settings")]
    public float flankOffset = 8f;
    private float flankAngle;

    private EnemyShooting shooting;
    private Rigidbody2D rb;
    private NavMeshAgent agent;

    [Header("Behavior Settings")]
    public float aggroKeepDistance = 8f; 

    [Header("Acceleration Settings")]
    public float acceleration = 2f;
    public float deceleration = 3f;

    void Start()
    {
        shooting = GetComponent<EnemyShooting>();
        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();
        
        if (agent == null)
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
        }

        // Configure agent for 2D top-down
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = moveSpeed;
        agent.acceleration = acceleration;
        agent.stoppingDistance = stoppingDistance;
        
        // Ensure Rigidbody2D is Kinematic to avoid conflict with NavMeshAgent
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        flankAngle = Random.Range(0, 360);
        
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
        }
    }

    void Update()
    {
        if (target == null || agent == null) return;

        float distance = Vector2.Distance(transform.position, target.position);

        // Face player (body rotation)
        Vector2 direction = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle), 5f * Time.deltaTime);

        if (type == EnemyType.Agro)
        {
            if (distance > stoppingDistance)
            {
                agent.isStopped = false;
                agent.SetDestination(target.position);
            }
            else if (distance < aggroKeepDistance)
            {
                // Back up: find a point away from the player
                Vector3 awayPos = transform.position - (Vector3)direction * 2f;
                agent.isStopped = false;
                agent.SetDestination(awayPos);
            }
            else
            {
                agent.isStopped = true;
            }
        }
        else // Flanking
        {
            // Target the rear of the player
            Vector3 rearPos = target.position - target.right * flankOffset;
            
            flankAngle += Time.deltaTime * 20f;
            Vector3 sideOffset = target.up * Mathf.Sin(flankAngle * Mathf.Deg2Rad) * (flankOffset * 0.5f);
            
            Vector3 targetPos = rearPos + sideOffset;
            
            agent.isStopped = false;
            agent.SetDestination(targetPos);
        }

        if (distance <= shootingRange)
        {
            if (shooting != null) shooting.TryShoot();
        }
    }
}
