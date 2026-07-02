using UnityEngine;

public class TankHealth : MonoBehaviour
{
    public GameData dataGame; // 1. Tarik file asset GameData ke sini di Inspector
    public bool isPlayer = true; // 2. Centang ini di Inspector jika script ini menempel pada tank Player

    [Header("Status Tank")]
    public int maxHealth = 100;
    private int currentHealth;
    private bool isDead = false;

    [Header("UI Health Bar")]
    public Healthbar healthBar; 

    [Header("Efek Visual")]
    public GameObject explosionPrefab;
    public Animator anim; 

    [Header("Audio")] 
    public AudioSource engineSound; 
    public AudioSource explosionSound; 

    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;

    void Start()
{
    // Jika ini adalah tank player dan file GameData sudah dimasukkan
    if (isPlayer && dataGame != null)
    {
        // PENCEGAHAN: Jika data kesehatan di data pusat kosong/0, baru set ke penuh
        if (dataGame.currentHealth <= 0) 
        {
            dataGame.currentHealth = maxHealth;
        }

        // AMBIL DARAH TERAKHIR dari asset data pusat (yang bernilai 55 itu)
        currentHealth = dataGame.currentHealth; 
    }
    else
    {
        // Jika ini tank musuh di Stage 2, berikan darah penuh secara default
        currentHealth = maxHealth; 
    }

    // UPDATE visual UI Slider darah agar tidak tampil penuh secara salah
    if (healthBar != null)
    {
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(currentHealth); // Ini yang akan memaksa slider mencerminkan angka 55
    }
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
        
        // 4. Jika Player terluka, update datanya ke GameData pusat
        if (isPlayer && dataGame != null)
        {
            dataGame.currentHealth = currentHealth;
        }

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

        // Jika Player mati, reset darah di GameData ke max untuk game berikutnya
        if (isPlayer && dataGame != null)
        {
            dataGame.currentHealth = maxHealth;
        }

        // --- SISA SKRIP DIE() TETAP SAMA SEPERTI MILIKMU ---
        PlayerTank playerScript = GetComponent<PlayerTank>();
        if (playerScript != null) playerScript.enabled = false;

        TankShooting shootingScript = GetComponent<TankShooting>();
        if (shootingScript != null) shootingScript.enabled = false;
        
        Tiger2Turret playerTurret = GetComponentInChildren<Tiger2Turret>();
        if (playerTurret != null) playerTurret.enabled = false;

        EnemyTurret enemyTurret = GetComponentInChildren<EnemyTurret>();
        if (enemyTurret != null) enemyTurret.enabled = false;

        EnemyAI enemyAI = GetComponent<EnemyAI>();
        if (enemyAI != null) enemyAI.enabled = false;

        EnemyShooting enemyShooting = GetComponent<EnemyShooting>();
        if (enemyShooting != null) enemyShooting.enabled = false;

        if (TryGetComponent(out Collider2D tankCollider)) { tankCollider.enabled = false; }

        if (explosionPrefab != null) Instantiate(explosionPrefab, transform.position, transform.rotation);
        if (anim != null) { anim.enabled = true; anim.SetTrigger("Die"); }
        if (engineSound != null) { engineSound.Stop(); }
        if (explosionSound != null) { explosionSound.Play(); }

        if (playerScript == null)
        {
            SpriteRenderer[] srs = GetComponentsInChildren<SpriteRenderer>();
            foreach (var s in srs) { s.color = Color.black; }
        }
    }
}