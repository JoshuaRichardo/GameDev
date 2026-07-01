using UnityEngine;
using UnityEngine.SceneManagement; // Wajib ditambahkan agar sistem tahu kita di map mana

public class CheckpointManager : MonoBehaviour
{
    [Header("Tulis penanda untuk Console (Misal: Level 2)")]
    public string namaLevel;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Mengecek apakah yang masuk ke dalam radius adalah tank pemain
        if (collision.CompareTag("Player"))
        {
            // 1. Menyimpan status checkpoint
            PlayerPrefs.SetInt("HasCheckpoint", 1);
            
            // 2. Menyimpan koordinat X dan Y
            PlayerPrefs.SetFloat("CheckpointX", transform.position.x);
            PlayerPrefs.SetFloat("CheckpointY", transform.position.y);
            
            // 3. BARIS PALING PENTING: Menyimpan nama Map/Scene tempat menara ini berada!
            PlayerPrefs.SetString("LevelCheckpoint", SceneManager.GetActiveScene().name);
            
            PlayerPrefs.Save();

            // Munculkan pesan sesuai nama level yang kamu ketik di Inspector
            Debug.Log($"🔥 SISTEM: Checkpoint {namaLevel} Tersimpan!");
        }
    }
}