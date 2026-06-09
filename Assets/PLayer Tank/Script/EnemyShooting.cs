using UnityEngine;
using System.Collections;

public class EnemyShooting : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 7f; // Reload time between shots (seconds)
    private bool canShoot = true;

    [Header("Effects")]
    public ParticleSystem muzzleFlash;
    public ParticleSystem smokeEffect;

    [Header("Audio")]
    public AudioSource shootSound;

    [Header("Combat Settings")]
    public float missChance = 0.35f; // 35% chance to miss
    public int bulletDamage = 25; // 4 hits to kill player (100 HP)

    public void TryShoot()
    {
        if (canShoot && gameObject.activeInHierarchy)
        {
            if (!IsFriendlyInWay())
            {
                StartCoroutine(ShootRoutine());
            }
        }
    }

    private bool IsFriendlyInWay()
    {
        if (firePoint == null) return false;

        // Raycast in the direction of fire to check for other enemies
        float rayDistance = 10f; // Check up to 10 units
        int layerMask = ~LayerMask.GetMask("Bullet"); // Ignore bullets

        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, firePoint.right, rayDistance, layerMask);

        if (hit.collider != null)
        {
            // Check if we hit another enemy
            // We can check if the object has an EnemyAI component
            if (hit.collider.gameObject != gameObject && hit.collider.GetComponentInParent<EnemyAI>() != null)
            {
                return true;
            }
        }

        return false;
    }

    IEnumerator ShootRoutine()
    {
        canShoot = false;

        if (bulletPrefab != null && firePoint != null)
        {
            // Apply accuracy/miss chance
            Quaternion fireRotation = firePoint.rotation;
            if (Random.value < missChance)
            {
                // Add significant spread to "miss"
                float spread = Random.Range(15f, 30f) * (Random.value > 0.5f ? 1 : -1);
                fireRotation *= Quaternion.Euler(0, 0, spread);
            }

            GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, fireRotation);
            Bullet bullet = bulletObj.GetComponent<Bullet>();
            if (bullet != null) 
            {
                bullet.shooter = gameObject;
                bullet.damage = bulletDamage;
            }

            if (muzzleFlash != null)
            {
                ParticleSystem mf = Instantiate(muzzleFlash, firePoint.position, firePoint.rotation);
                var renderer = mf.GetComponent<ParticleSystemRenderer>();
                if (renderer != null)
                {
                    renderer.sortingLayerName = "Turet";
                    renderer.sortingOrder = 100;
                }
            }

            if (smokeEffect != null)
            {
                ParticleSystem se = Instantiate(smokeEffect, firePoint.position, firePoint.rotation);
                var renderer = se.GetComponent<ParticleSystemRenderer>();
                if (renderer != null)
                {
                    renderer.sortingLayerName = "Turet";
                    renderer.sortingOrder = 100;
                }
            }
        }

        if (shootSound != null) shootSound.Play();

        yield return new WaitForSeconds(fireRate);
        canShoot = true;
    }
}
