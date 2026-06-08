using UnityEngine;

public class TankHealth : MonoBehaviour
{
    [Header("Status Tank")]
    public int maxHealth = 100;
    private int currentHealth;
    private bool isDead = false;

    [Header("UI Health Bar")]
    public Healthbar healthBar; // Tarik objek UI HealthBar ke sini nanti

    [Header("Efek Visual")]
    public GameObject explosionPrefab;
    public Animator anim; 

    [Header("Audio")] 
    public AudioSource engineSound; 
    public AudioSource explosionSound; 

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
    }

    void Update()
    {
        // Tombol darurat K untuk ngetes mati
        if (Input.GetKeyDown(KeyCode.K) && !isDead)
        {
            Die();
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        if (healthBar != null)
        {
            healthBar.SetHealth(0);
        }

        // --- GABUNGAN SKRIP UNTUK MEMATIKAN KOMPONEN (PLAYER & ENEMY) ---
        
        // Mematikan skrip pergerakan & menembak milik Player
        PlayerTank playerScript = GetComponent<PlayerTank>();
        if (playerScript != null) playerScript.enabled = false;

        TankShooting shootingScript = GetComponent<TankShooting>();
        if (shootingScript != null) shootingScript.enabled = false;
        
        // Mematikan skrip turet (baik punya Player maupun Musuh)
        Tiger2Turret playerTurret = GetComponentInChildren<Tiger2Turret>();
        if (playerTurret != null) playerTurret.enabled = false;

        EnemyTurret enemyTurret = GetComponentInChildren<EnemyTurret>();
        if (enemyTurret != null) enemyTurret.enabled = false;

        // Mematikan skrip milik Musuh (AI & nembak)
        EnemyAI enemyAI = GetComponent<EnemyAI>();
        if (enemyAI != null) enemyAI.enabled = false;

        EnemyShooting enemyShooting = GetComponent<EnemyShooting>();
        if (enemyShooting != null) enemyShooting.enabled = false;

        // Mematikan Collider agar tank hancur tidak bisa ditabrak lagi
        if (TryGetComponent(out Collider2D tankCollider)) { tankCollider.enabled = false; }

        // Efek Ledakan
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, transform.rotation);
        }

        // Animasi Mati
        if (anim != null)
        {
            anim.enabled = true; 
            anim.SetTrigger("Die"); 
        }

        // Mengatur Audio (Matikan mesin, mainkan ledakan)
        if (engineSound != null) { engineSound.Stop(); }
        if (explosionSound != null) { explosionSound.Play(); }

        // Jika ini bukan player (melainkan musuh), hancurkan object setelah 2 detik
        if (playerScript == null)
        {
            Destroy(gameObject, 2f);
        }
    }
}