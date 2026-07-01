using UnityEngine;
using UnityEngine.SceneManagement; 

public class PindahScene : MonoBehaviour
{
    [Header("Ketik Nama Scene Selanjutnya di Bawah:")]
    public string namaSceneTujuan;

    [Header("Pengaturan Progress Bar (Map 1 = 1, Map 2 = 2, Map 3 = 3):")]
    public int nomorMapTujuan; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Mengecek apakah yang menyentuh zona ini adalah tank pemain
        if (collision.CompareTag("Player"))
        {
            // Mengamankan data indeks map ke memori internal bawaan Unity
            PlayerPrefs.SetInt("CurrentMapIndex", nomorMapTujuan);
            PlayerPrefs.Save();

            Debug.Log("Meninggalkan area... Memuat " + namaSceneTujuan);
            
            // Memuat scene baru
            SceneManager.LoadScene(namaSceneTujuan);
        }
    }
}