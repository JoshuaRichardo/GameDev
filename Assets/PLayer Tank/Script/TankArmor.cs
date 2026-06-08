using UnityEngine;
using System.Collections;

public class TankArmor : MonoBehaviour
{
    [Header("Armor Settings (Shots to kill if damage is 17)")]
    public float frontMultiplier = 0.5f; // 17 * 0.5 = 8.5 (100 / 8.5 = 11.76 shots)
    public float sideMultiplier = 2.0f;  // 17 * 2 = 34 (100 / 34 = 2.94 shots)
    
    [Header("Immobilization")]
    public float stunDuration = 10f;
    private bool isStunned = false;

    private TankHealth health;
    private PlayerTank playerMovement;

    void Awake()
    {
        health = GetComponent<TankHealth>();
        playerMovement = GetComponent<PlayerTank>();
    }

    public void OnHit(Vector2 bulletDirection, int baseDamage)
    {
        if (isStunned) return; // Optional: can you take damage while stunned? User didn't specify. Assuming yes.

        // Calculate hit angle relative to tank facing
        // transform.right is the front of the tank in this project
        Vector2 tankForward = transform.right;
        Vector2 hitDirection = -bulletDirection.normalized; // Direction from which bullet came
        
        float angle = Vector2.Angle(tankForward, hitDirection);

        if (angle < 45f) // Frontal hit (cone of 90 degrees total)
        {
            ApplyDamage(baseDamage * frontMultiplier);
        }
        else if (angle > 135f) // Rear hit
        {
            StartCoroutine(StunRoutine());
            // User said "if shot from behind and hit tank butt tank will be unable to move for 10 seconds"
            // Didn't specify damage, assuming low or no damage? Let's give no damage for now or very little.
        }
        else // Side hit
        {
            ApplyDamage(baseDamage * sideMultiplier);
        }
    }

    private void ApplyDamage(float damage)
    {
        if (health != null)
        {
            health.TakeDamage(Mathf.RoundToInt(damage));
        }
    }

    private IEnumerator StunRoutine()
    {
        if (isStunned) yield break;
        isStunned = true;

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        yield return new WaitForSeconds(stunDuration);

        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }
        
        isStunned = false;
    }
}
