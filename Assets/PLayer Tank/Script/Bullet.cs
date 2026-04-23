using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 40f; // Peluru tank asli sangat cepat
    public float lifeTime = 3f;

    [Header("Effects")]
    public GameObject impactEffectPrefab; // Prefab ledakan kecil/percikan api
    public TrailRenderer bulletTrail;    // Tarik Trail Renderer ke sini

    void Start()
    {
        // Hancurkan peluru otomatis jika tidak mengenai apapun
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Bergerak maju berdasarkan arah lokal peluru (sumbu X positif)
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Jangan menabrak tank sendiri atau sesama peluru
        if (collision.CompareTag("Player") || collision.CompareTag("Bullet")) return;

        // 1. Munculkan efek ledakan di titik benturan
        if (impactEffectPrefab != null)
        {
            Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);
        }

        // 2. Logika Damage (Opsional: Jika target punya script Health)
        // collision.GetComponent<EnemyHealth>()?.TakeDamage(50);

        Debug.Log("Impact: " + collision.name);

        // 3. Hancurkan peluru setelah menabrak
        Destroy(gameObject);
    }
}