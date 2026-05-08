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

        GetComponent<PlayerTank>().enabled = false; 
        GetComponent<TankShooting>().enabled = false;
        
        Tiger2Turret turretScript = GetComponentInChildren<Tiger2Turret>();
        if (turretScript != null) turretScript.enabled = false;

        Collider2D tankCollider = GetComponent<Collider2D>();
        if (tankCollider != null) tankCollider.enabled = false;

        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
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
    }
}