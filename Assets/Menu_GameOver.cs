using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuGameOver : MonoBehaviour
{
    [Header("Ketik Nama Scene Level 1 (Misal: SampleScene)")]
    public string namaSceneAwal = "SampleScene";

    public void FungsiTryAgain()
    {
        // 1. Hapus memori checkpoint agar pemain benar-benar mengulang dari titik nol
        PlayerPrefs.DeleteKey("HasCheckpoint");
        PlayerPrefs.Save();

        // 2. Pastikan waktu game berjalan normal (berjaga-jaga jika sempat ter-pause)
        Time.timeScale = 1f;

        // 3. Pindah ke level 1
        SceneManager.LoadScene(namaSceneAwal);
    }

    public void FungsiExit()
    {
        Debug.Log("Sistem: Keluar dari game!");
        Application.Quit(); // Ini akan menutup game saat dimainkan dalam bentuk .exe nanti
    }
}