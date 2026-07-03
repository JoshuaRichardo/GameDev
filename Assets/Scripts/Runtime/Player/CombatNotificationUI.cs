using UnityEngine;
using TMPro;

/// <summary>
/// Combat HUD driver: shows how many times each side of the player tank has been
/// shot, plus a transient "engine hit" notification with a live immobilize countdown.
/// Reads its data from the player's <see cref="TankArmor"/>.
/// </summary>
public class CombatNotificationUI : MonoBehaviour
{
    [Header("Source")]
    public TankArmor armor;

    [Header("UI References")]
    public TextMeshProUGUI shotCountText;
    public TextMeshProUGUI engineNotificationText;

    void Start()
    {
        if (armor == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) armor = player.GetComponent<TankArmor>();
        }

        if (engineNotificationText != null)
            engineNotificationText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (armor == null) return;

        if (shotCountText != null)
        {
            shotCountText.text =
                "Front has been shot " + armor.FrontHits + " times\n" +
                "Left has been shot "  + armor.LeftHits  + " times\n" +
                "Right has been shot " + armor.RightHits + " times\n" +
                "Rear has been shot "  + armor.RearHits  + " times";
        }

        if (engineNotificationText != null)
        {
            if (armor.IsStunned)
            {
                if (!engineNotificationText.gameObject.activeSelf)
                    engineNotificationText.gameObject.SetActive(true);

                int seconds = Mathf.CeilToInt(armor.StunTimeRemaining);
                engineNotificationText.text = "ENGINE HIT! Cannot move for " + seconds + "s";
            }
            else if (engineNotificationText.gameObject.activeSelf)
            {
                engineNotificationText.gameObject.SetActive(false);
            }
        }
    }
}
