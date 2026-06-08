using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthHUD : MonoBehaviour
{
    public TankHealth playerHealth;
    public UnityEngine.UI.Slider healthSlider;
    public TMPro.TextMeshProUGUI healthText;

    void Start()
    {
        if (playerHealth == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerHealth = player.GetComponent<TankHealth>();
            }
        }
    }

    void Update()
    {
        if (playerHealth != null)
        {
            float current = playerHealth.GetCurrentHealth();
            float max = playerHealth.GetMaxHealth();
            
            if (healthSlider != null)
            {
                healthSlider.maxValue = max;
                healthSlider.value = current;
            }

            if (healthText != null)
            {
                healthText.text = string.Format("Health: {0}/{1}", current, max);
            }
        }
    }
}
