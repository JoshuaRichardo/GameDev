using UnityEngine;

public class TankHealth : MonoBehaviour
{
    [Header("Status Tank")]
    public int maxHealth = 100;
    private int currentHealth;
    private bool isDead = false;

    [Header("Efek Visual")]
    public GameObject explosionPrefab;
    
    // UBAH JADI PUBLIC agar bisa ditarik langsung dari Inspector
    public Animator anim; 

    [Header("Audio")] 
    public AudioSource engineSound; 
    public AudioSource explosionSound; // TAMBAHAN BARU: Variabel untuk suara ledakan

    void Start()
    {
        currentHealth = maxHealth;
        // Kita tidak perlu lagi GetComponent<Animator>() di sini
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K) && !isDead)
        {
            Die();
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        PlayerTank playerScript = GetComponent<PlayerTank>();
        if (playerScript != null) playerScript.enabled = false;

        TankShooting shootingScript = GetComponent<TankShooting>();
        if (shootingScript != null) shootingScript.enabled = false;
        
        // Coba cari script turet apa pun
        Tiger2Turret playerTurret = GetComponentInChildren<Tiger2Turret>();
        if (playerTurret != null) playerTurret.enabled = false;

        EnemyTurret enemyTurret = GetComponentInChildren<EnemyTurret>();
        if (enemyTurret != null) enemyTurret.enabled = false;

        EnemyAI enemyAI = GetComponent<EnemyAI>();
        if (enemyAI != null) enemyAI.enabled = false;

        EnemyShooting enemyShooting = GetComponent<EnemyShooting>();
        if (enemyShooting != null) enemyShooting.enabled = false;

        Collider2D tankCollider = GetComponent<Collider2D>();
        if (tankCollider != null) tankCollider.enabled = false;

        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, transform.rotation);
        }

        // Animator yang tadi mati, kita nyalakan SATU DETIK sebelum mati
        if (anim != null)
        {
            anim.enabled = true; 
            anim.SetTrigger("Die"); 
        }

        // TAMBAHKAN INI UNTUK MEMATIKAN SUARA MESIN SAAT HANCUR
        if (engineSound != null)
        {
            engineSound.Stop();
        }

        // TAMBAHAN BARU: Mainkan suara ledakan saat tank hancur
        if (explosionSound != null)
        {
            explosionSound.Play();
        }

        // Jika ini bukan player, mungkin mau hancurkan object setelah beberapa saat
        if (playerScript == null)
        {
            Destroy(gameObject, 2f);
        }
    }
}