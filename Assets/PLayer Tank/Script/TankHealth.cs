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

        // PENGAMAN: Skrip di bawah ini hanya akan dimatikan JIKA komponennya memang ada di Tank.
        // Ini mencegah "compiler error" atau "null reference" jika nama skrip berbeda.
        if (TryGetComponent(out MonoBehaviour playerTank)) { playerTank.enabled = false; }
        if (TryGetComponent(out MonoBehaviour tankShooting)) { tankShooting.enabled = false; }
        
        // Mematikan skrip turret di anak (child) objek jika ada
        MonoBehaviour turretScript = GetComponentInChildren<MonoBehaviour>();
        if (turretScript != null && turretScript.GetType().Name == "Tiger2Turret") 
        { 
            turretScript.enabled = false; 
        }

        // Mematikan Collider agar tank hancur tidak bisa ditabrak lagi
        if (TryGetComponent(out Collider2D tankCollider)) { tankCollider.enabled = false; }

        // Efek Ledakan
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        // Animasi Mati
        if (anim != null)
        {
            anim.enabled = true; 
            anim.SetTrigger("Die"); 
        }

        // Mengatur Audio
        if (engineSound != null) { engineSound.Stop(); }
        if (explosionSound != null) { explosionSound.Play(); }
    }
}