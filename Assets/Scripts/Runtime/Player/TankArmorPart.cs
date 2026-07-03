using UnityEngine;

/// <summary>
/// One of the 4 directional trigger zones (Front / Left / Right / Rear) covering the tank body.
/// Detects bullets entering its zone and reports the hit to the central <see cref="TankArmor"/>.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class TankArmorPart : MonoBehaviour
{
    public TankArmor.ArmorSide side;

    private TankArmor armor;

    void Awake()
    {
        armor = GetComponentInParent<TankArmor>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (armor == null) return;

        Bullet bullet = other.GetComponent<Bullet>();
        if (bullet == null) bullet = other.GetComponentInParent<Bullet>();
        if (bullet == null) return;

        // Ignore bullets fired by this very tank.
        if (bullet.shooter == armor.gameObject) return;

        // A single bullet only registers one hit (avoids double counting at corners).
        if (bullet.consumed) return;
        bullet.consumed = true;

        armor.RegisterHit(side, bullet.damage);
        bullet.Detonate();
    }
}
