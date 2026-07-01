using UnityEngine;
using UnityEngine.UI;

public class TankProgressBar : MonoBehaviour
{
    public Image fillImage; // Masukkan file fil;.png ke sini
    
    private int mapIndex = 0;
    private float progressLokal = 0f; 

    void Update()
    {
        // 1. Selalu ambil nomor map tujuan terbaru dari memori internal Unity setiap frame
        mapIndex = PlayerPrefs.GetInt("CurrentMapIndex", 0);

        // 2. Hitung pembagian progress bar (tiap map jatahnya 25% atau 0.25f)
        float batasBawahProgress = mapIndex * 0.25f;
        float kontribusiMapIni = progressLokal * 0.25f;
        float totalProgressAkhir = batasBawahProgress + kontribusiMapIni;

        // 3. Terapkan ke fillAmount gambar fil;.png
        fillImage.fillAmount = Mathf.Clamp01(totalProgressAkhir);
    }

    // Fungsi ini dipanggil terus oleh skrip 'jarakmap' di Tank saat berjalan
    public void UpdateLocalProgress(float persen)
    {
        progressLokal = persen;
    }
}