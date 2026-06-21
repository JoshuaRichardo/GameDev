using UnityEngine;
using UnityEngine.SceneManagement; // Ini wajib dipanggil untuk sistem perpindahan map

public class PindahScene : MonoBehaviour
{
    [Header("Ketik Nama Scene Selanjutnya di Bawah:")]
    public string namaSceneTujuan;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Mengecek apakah yang menyentuh zona ini adalah tank pemain
        if (collision.CompareTag("Player"))
        {
            // Menghapus memori checkpoint dari level sebelumnya agar di level baru mulai dari titik awal
            PlayerPrefs.DeleteKey("HasCheckpoint"); 
            
            Debug.Log("Meninggalkan area... Memuat " + namaSceneTujuan);
            
            // Memuat scene baru sesuai nama yang kamu ketik
            SceneManager.LoadScene(namaSceneTujuan);
        }
    }
}