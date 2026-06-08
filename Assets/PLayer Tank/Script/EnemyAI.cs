using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyType { Agro, Flanking }
    public EnemyType type = EnemyType.Agro;

    public Transform target;
    public float stoppingDistance = 10f;
    public float moveSpeed = 1.8f; // Reduced by 40% from 3f
    public float shootingRange = 15f;
    
    [Header("Flanking Settings")]
    public float flankOffset = 8f;
    private float flankAngle;

    private EnemyShooting shooting;
    private Rigidbody2D rb;

    void Start()
    {
        shooting = GetComponent<EnemyShooting>();
        rb = GetComponent<Rigidbody2D>();
        flankAngle = Random.Range(0, 360);
        
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
        }
    }

    void Update()
    {
        if (target == null) return;

        float distance = Vector2.Distance(transform.position, target.position);

        // Face player (body)
        Vector2 direction = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle), 5f * Time.deltaTime);

        if (type == EnemyType.Agro)
        {
            if (distance > stoppingDistance)
            {
                transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
            }
        }
        else // Flanking
        {
            // Move towards a point around the player
            flankAngle += Time.deltaTime * 10f; // Rotate around player slowly
            Vector3 flankPos = target.position + new Vector3(Mathf.Cos(flankAngle * Mathf.Deg2Rad), Mathf.Sin(flankAngle * Mathf.Deg2Rad), 0) * flankOffset;
            
            transform.position = Vector2.MoveTowards(transform.position, flankPos, moveSpeed * Time.deltaTime);
        }

        if (distance <= shootingRange)
        {
            if (shooting != null) shooting.TryShoot();
        }
    }
}
