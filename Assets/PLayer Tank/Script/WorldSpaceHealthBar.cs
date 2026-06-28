using UnityEngine;

/// <summary>
/// Attach to the root GameObject of a World Space Canvas that is parented to
/// the player tank.  Every LateUpdate it:
///   1. Repositions itself at a fixed world-space offset above the parent.
///   2. Resets its rotation to Quaternion.identity so the health bar stays
///      perfectly horizontal regardless of how the tank is turning.
///
/// Setup: Player (TankHealth) ──► HealthBarCanvas (this script + Canvas)
///                                    └── Slider (Healthbar.cs)
/// </summary>
public class WorldSpaceHealthBar : MonoBehaviour
{
    [Tooltip("World-space offset from the tank's pivot. " +
             "Increase Y to push the bar higher above the hull.")]
    public Vector3 offset = new Vector3(0f, 1.5f, 0f);

    private Transform parentTransform;

    void Awake()
    {
        parentTransform = transform.parent;

        if (parentTransform == null)
            Debug.LogWarning("[WorldSpaceHealthBar] This GameObject has no parent. " +
                             "It must be a child of the player tank.", this);
    }

    /// <summary>
    /// LateUpdate runs after all Update() calls, so the parent's position is
    /// already settled for this frame before we reposition the bar.
    /// </summary>
    void LateUpdate()
    {
        if (parentTransform == null) return;

        // Follow the parent in world space with the configured offset.
        transform.position = parentTransform.position + offset;

        // Counteract any rotation inherited from the rotating tank body
        // so the bar is always axis-aligned in the world.
        transform.rotation = Quaternion.identity;
    }
}
