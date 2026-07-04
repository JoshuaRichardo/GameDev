using UnityEngine;

public class FinishZone : MonoBehaviour
{
    [Header("Masukkan Panel_MenuVictory ke sini")]
    public GameObject layarMenuVictory;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Deteksi apakah objek yang menyentuh memiliki Tag "Player" (Tank Pemain)
        if (collision.CompareTag("Player"))
        {
            FungsiVictory();
        }
    }

    private void FungsiVictory()
    {
        if (layarMenuVictory != null)
        {
            layarMenuVictory.SetActive(true); // Memunculkan menu kemenangan
        }
        
        Time.timeScale = 0f; // Membekukan waktu agar semua pergerakan game berhenti
        Debug.Log("Tank sampai di Titik Parkir. Kamu Menang!");
    }
}