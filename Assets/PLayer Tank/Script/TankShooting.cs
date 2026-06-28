using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class TankShooting : MonoBehaviour
{
    private InputMaster controls;

    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 4f; // Jeda antar tembakan (detik)
    private bool canShoot = true;

    [Header("Recoil Settings (Realistic)")]
    public Transform tankBody; // TARIK OBJECT 'BODY' KE SINI DI INSPECTOR
    public float recoilForce = 0.3f; // Jarak sentakan mundur
    public float recoilSpeed = 15f; // Kecepatan sentakan

    [Header("Effects")]
    public ParticleSystem muzzleFlash;
    public ParticleSystem smokeEffect;

    [Header("Audio")] // TAMBAHKAN INI
    public AudioSource shootSound; // TAMBAHKAN INI

    public int bulletDamage = 100; // One hit to kill enemy

    private void Awake() => controls = new InputMaster();
    private void OnEnable() => controls.Player.Enable();
    private void OnDisable() => controls.Player.Disable();

    private void Start()
    {
        // Pastikan nama "Attack" sesuai dengan di InputMaster kamu
        controls.Player.Attack.performed += _ => TryShoot();
    }

    void TryShoot()
    {
        if (canShoot)
        {
            StartCoroutine(ShootRoutine());
        }
    }

    IEnumerator ShootRoutine()
    {
        canShoot = false;

        // 1. Munculkan Peluru
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Bullet bulletScript = bulletObj.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.shooter = gameObject;
                bulletScript.damage = bulletDamage;
            }
            
            ParticleSystem mf = Instantiate(muzzleFlash, firePoint.position, firePoint.rotation);
            if (mf != null)
            {
                var renderer = mf.GetComponent<ParticleSystemRenderer>();
                if (renderer != null)
                {
                    renderer.sortingLayerName = "Turet";
                    renderer.sortingOrder = 100; // Tetap 100 agar di atas peluru (90)
                }
            }

            ParticleSystem se = Instantiate(smokeEffect, firePoint.position, firePoint.rotation);
            if (se != null)
            {
                var renderer = se.GetComponent<ParticleSystemRenderer>();
                if (renderer != null)
                {
                    renderer.sortingLayerName = "Turet";
                    renderer.sortingOrder = 100;
                }
            }
        }

        // 2. Efek Visual
        if (muzzleFlash != null) muzzleFlash.Play();
        if (smokeEffect != null) smokeEffect.Play();

        // TAMBAHKAN BAGIAN INI UNTUK MEMUTAR SUARA
        if (shootSound != null)
        {
            shootSound.Play();
        }

        // 3. Efek Mundur Realistik
        StartCoroutine(ApplyRealRecoil());

        //FDebug.Log("Tiger 2 Menembak! Reloading...");

        // 4. Jeda Tembak (Reload)
        yield return new WaitForSeconds(fireRate);
        canShoot = true;
        //Debug.Log("Ready to Fire!");
    }

    IEnumerator ApplyRealRecoil()
    {
        if (tankBody == null)
        {
            //Debug.LogError("Tarik object 'Body' ke kolom Tank Body di Inspector!");
            yield break;
        }

        Vector3 startPos = tankBody.position;

        // FIX: Menggunakan arah hadap firePoint untuk menentukan arah mundur
        // transform.right adalah arah "depan" untuk sprite 2D yang menghadap kanan
        Vector3 recoilDirection = -firePoint.right;

        Vector3 targetPos = startPos + (recoilDirection * recoilForce);

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * recoilSpeed;
            // Pindahkan badan tank ke arah berlawanan tembakan
            tankBody.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        //Debug.Log("Recoil Selesai");
    }
}