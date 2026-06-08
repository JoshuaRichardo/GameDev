using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 40f; 
    public float lifeTime = 3f;
    public float maxRange = 50f; // Jarak maksimal peluru sebelum meledak

    [Header("Effects")]
    public GameObject impactEffectPrefab; 
    public TrailRenderer bulletTrail;    

    private Vector3 startPosition;

    public int damage = 100; // Default 100 to kill enemy in one hit
    public GameObject shooter; // Simpan siapa yang menembak agar tidak menabrak diri sendiri

    void Start()
    {
        startPosition = transform.position;

        // FIX: Pastikan peluru berada di paling depan (on top) tapi di bawah muzzle flash
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingLayerName = "Turet"; // Layer tertinggi
            sr.sortingOrder = 90; // Di bawah muzzle flash (100)
        }
        if (bulletTrail != null)
        {
            bulletTrail.sortingLayerName = "Turet";
            bulletTrail.sortingOrder = 90;
        }

        // Hancurkan peluru otomatis jika tidak mengenai apapun dalam waktu tertentu
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Bergerak maju berdasarkan arah lokal peluru (sumbu X positif)
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        // RANGE CHECK: Jika jarak sudah melebihi maxRange, meledak
        if (Vector3.Distance(startPosition, transform.position) >= maxRange)
        {
            Explode();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Jangan meledak jika menabrak shooter sendiri, sesama peluru, Tanah, bagian Turet, atau Batas Map
        if (collision.gameObject == shooter || 
            collision.CompareTag("Bullet") || 
            collision.CompareTag("Ground") || 
            collision.CompareTag("Batas") ||
            collision.gameObject.layer == LayerMask.NameToLayer("Turet") ||
            collision.gameObject.layer == LayerMask.NameToLayer("Body")) 
        {
            return;
        }

        // Deal damage if hit something with TankHealth
        TankHealth health = collision.GetComponent<TankHealth>();
        if (health == null)
        {
            // Check in parent in case collider is on a child object
            health = collision.GetComponentInParent<TankHealth>();
        }

        if (health != null)
        {
            health.TakeDamage(damage);
        }

        // Meledak jika menabrak apa pun selain di atas (Tembok, Musuh, dll)
        Explode();
    }

    private void Explode()
    {
        // Munculkan efek ledakan
        if (impactEffectPrefab != null)
        {
            GameObject effect = Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);
            
            // FIX: Pastikan efek ledakan juga berada di paling depan
            SpriteRenderer effectSR = effect.GetComponent<SpriteRenderer>();
            if (effectSR != null)
            {
                effectSR.sortingLayerName = "Turet";
                effectSR.sortingOrder = 100;
            }

            ParticleSystem ps = effect.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                var renderer = ps.GetComponent<ParticleSystemRenderer>();
                if (renderer != null)
                {
                    renderer.sortingLayerName = "Turet";
                    renderer.sortingOrder = 100;
                }
            }

            // Hancurkan objek efek setelah 2 detik agar tidak menumpuk
            Destroy(effect, 2f);
        }

        // Hancurkan peluru
        Destroy(gameObject);
    }
}
