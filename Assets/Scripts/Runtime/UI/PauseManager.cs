using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("Masukkan Panel_MenuPause ke sini")]
    public GameObject layarMenuPause; 

    public void FungsiPause()
    {
        layarMenuPause.SetActive(true); // Memunculkan menu gelap
        Time.timeScale = 0f;            // Membekukan waktu (semua tank berhenti)
    }

    public void FungsiLanjut()
    {
        layarMenuPause.SetActive(false); // Menyembunyikan menu
        Time.timeScale = 1f;             // Menjalankan waktu kembali
    }

    public void FungsiKeluar()
    {
        Time.timeScale = 1f; // Sangat penting agar waktu normal lagi sebelum keluar
        Debug.Log("Tombol Exit Ditekan!"); 
        // Application.Quit(); // Hapus tanda // di depan baris ini kalau game sudah mau di-build/dijadikan .exe
    }

    public void FungsiRestart()
    {
        Time.timeScale = 1f; // PENTING: Kembalikan waktu ke normal agar game tidak membeku saat load
        
        // Pilihan A: Jika nama scene Stage 1 kamu bernama "SampleScene" (sesuai gambar pertama)
        SceneManager.LoadScene("SampleScene"); 

        // Pilihan B: Jika kamu ingin otomatis reload Scene yang sedang aktif saat itu secara dinamis
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}