using UnityEngine;

public class ResetDataAwal : MonoBehaviour
{
    [Header("Tarik File Asset PusatDataGame ke Sini")]
    public GameData dataGame;

    [Header("Pengaturan Standar Awal Game")]
    public int darahAwal = 100;

    void Awake()
    {
        if (dataGame != null)
        {
            // Reset darah ke penuh
            dataGame.currentHealth = darahAwal;
            
            // Reset progress bar kembali ke map index paling pertama (Stage 1)
            dataGame.currentMapIndex = 0;

            Debug.Log("🔄 SISTEM: Data Game Terpusat Berhasil Di-reset Otomatis untuk Stage 1!");
        }
        else
        {
            Debug.LogError("❌ ERROR: File asset 'PusatDataGame' belum ditarik ke Inspector script ResetDataAwal!");
        }
    }
}