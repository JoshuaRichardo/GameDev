using UnityEngine;
using UnityEngine.UI;

public class TankProgressBar : MonoBehaviour
{
    public GameData dataGame; // Tarik file asset PusatDataGame di Inspector tiap scene
    public Image fillImage;   // Komponen gambar progress bar hijau
    
    private float progressLokal = 0f; 

    void Update()
    {
        if (dataGame == null || fillImage == null) return;

        // 1. Ambil indeks map saat ini (Stage 1 = 0, Stage 2 = 1, Stage 3 = 2, dst)
        int mapIndex = dataGame.currentMapIndex;

        // 2. Tentukan batas bawah dasar berdasarkan stage (Tiap stage berbobot maksimal 25% atau 0.25f)
        float batasBawahProgress = mapIndex * 0.25f;
        
        // 3. Tambahkan pergerakan berjalan tank di dalam map ini (maksimal berkontribusi sebesar 25%)
        float kontribusiMapIni = progressLokal * 0.25f;
        
        // 4. Gabungkan keduanya menjadi total progress akumulatif keseluruhan game
        float totalProgressAkhir = batasBawahProgress + kontribusiMapIni;

        // Tampilkan ke visual UI fill amount
        fillImage.fillAmount = Mathf.Clamp01(totalProgressAkhir);
    }

    // Fungsi ini sekarang AKTIF menerima kiriman angka (0.0 sampai 1.0) dari script jarakmap
    public void UpdateLocalProgress(float persen)
    {
        progressLokal = persen;
    }
}