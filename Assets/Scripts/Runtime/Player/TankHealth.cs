using UnityEngine;

public class TankHealth : MonoBehaviour
{
    public GameData dataGame; 
    public bool isPlayer = true; 

    [Header("Status Tank")]
    public int maxHealth = 100;
    private int currentHealth;
    private bool isDead = false;

    [Header("UI Health Bar")]
    public Healthbar healthBar; 

    // ==========================================
    // TAMBAHAN UNTUK PANEL GAME OVER
    // ==========================================
    [Header("UI Game Over")]
    public GameObject layarGameOver; // Tarik Panel_GameOver ke sini di Inspector

    [Header("Efek Visual")]
    public GameObject explosionPrefab;
    public Animator anim; 

    [Header("Audio")] 
    public AudioSource engineSound; 
    public AudioSource explosionSound; 
    public AudioSource musikLatarGame; // Tarik objek BGM/Backsound utama ke sini

    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;

    void Start()
    {
        if (isPlayer && dataGame != null)
        {
            if (dataGame.currentHealth <= 0) 
            {
                dataGame.currentHealth = maxHealth;
            }
            currentHealth = dataGame.currentHealth; 
        }
        else
        {
            currentHealth = maxHealth; 
        }

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetHealth(currentHealth); 
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

        if (isPlayer && dataGame != null)
        {
            dataGame.currentHealth = maxHealth;
        }

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

        // ========================================================
        // LOGIKA EKSTRA KHUSUS JIKA YANG MATI ADALAH PLAYER
        // ========================================================
        if (isPlayer)
        {
            // 1. Munculkan panel Game Over
            if (layarGameOver != null)
            {
                layarGameOver.SetActive(true);
            }

            // 2. Matikan backsound/musik latar utama game
            if (musikLatarGame != null)
            {
                musikLatarGame.Stop();
            }

            // 3. Bekukan waktu game (seperti sistem pause/victory)
            Time.timeScale = 0f;
        }
        else
        {
            // Jika yang mati adalah musuh, jalankan kode pewarnaan hitam milikmu sebelumnya
            SpriteRenderer[] srs = GetComponentsInChildren<SpriteRenderer>();
            foreach (var s in srs) { s.color = Color.black; }
        }
    }
}