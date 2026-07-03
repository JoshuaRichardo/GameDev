using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public void SetMaxHealth(float health)
    {
        if (slider != null)
        {
            slider.maxValue = health;
            // slider.value = health; // Hapus baris ini agar tidak memaksa slider penuh kembali saat pindah scene
        }
    }

    public void SetHealth(float health)
    {
        if (slider != null)
        {
            slider.value = health;
        }
    }
}