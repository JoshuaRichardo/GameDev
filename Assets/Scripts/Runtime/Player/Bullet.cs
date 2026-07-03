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

    [HideInInspector] public bool consumed = false; // set by a TankArmorPart so the hit is only counted once
    private bool exploded = false;

    void Start()
    {
        startPosition = transform.position;

        // The bullet is a kinematic trigger; enable full kinematic contacts so it
        // generates trigger events against other kinematic bodies (tank parts, enemies).
        var rb2d = GetComponent<Rigidbody2D>();
        if (rb2d != null) rb2d.useFullKinematicContacts = true;

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
        if (exploded) return;

        // Ignore the shooter and any of its child colliders (turret, parts, etc.)
        if (shooter != null && (collision.gameObject == shooter || collision.transform.IsChildOf(shooter.transform)))
        {
            return;
        }

        // Ignore other bullets, ground tiles and the map boundary.
        if (collision.CompareTag("Bullet") ||
            collision.CompareTag("Ground") ||
            collision.CompareTag("Batas"))
        {
            return;
        }

        // If this is one of the 4 directional armor zones, the TankArmorPart handles
        // both the damage and detonating this bullet — nothing to do here.
        if (collision.GetComponent<TankArmorPart>() != null)
        {
            return;
        }

        // If we hit the main body of a tank that uses the 4-part armor system,
        // ignore the main collider so the trigger parts remain the single source of truth.
        TankArmor armor = collision.GetComponent<TankArmor>();
        if (armor == null) armor = collision.GetComponentInParent<TankArmor>();
        if (armor != null && armor.useArmorParts)
        {
            return;
        }

        // Otherwise apply damage directly (enemies without an armor-part system, etc.)
        TankHealth health = collision.GetComponent<TankHealth>();
        if (health == null) health = collision.GetComponentInParent<TankHealth>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }

        Detonate();
    }

    /// <summary>Public entry point so a <see cref="TankArmorPart"/> can blow up this bullet.</summary>
    public void Detonate()
    {
        if (exploded) return;
        Explode();
    }

    private void Explode()
    {
        exploded = true;
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
