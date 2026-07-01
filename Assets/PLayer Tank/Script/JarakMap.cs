using UnityEngine;

public class jarakmap : MonoBehaviour
{
    public Transform titikStartMap;   // Batas Kiri Map
    public Transform titikPortalMap;  // Batas Kanan Map

    private Transform tankTransform;
    private TankProgressBar uiProgressBar;

    void Start()
    {
        // Otomatis mendeteksi Tank tempat skrip ini menempel
        tankTransform = this.transform;

        // Otomatis mencari progress bar di UI
        uiProgressBar = FindObjectOfType<TankProgressBar>();
    }

    void Update()
    {
        if (uiProgressBar == null || tankTransform == null || titikStartMap == null || titikPortalMap == null) 
            return;

        float startX = titikStartMap.position.x;
        float portalX = titikPortalMap.position.x;
        float tankX = tankTransform.position.x;

        // Hitung total lebar map (jarak batas kiri ke batas kanan)
        float totalLebarMap = portalX - startX;

        // Jika total lebar map minus atau nol, berarti posisi objek di Unity terbalik/salah koordinat
        if (totalLebarMap <= 0f)
        {
            // Balik rumusnya jika Anda meletakkan posisi portal di kiri dan start di kanan
            totalLebarMap = startX - portalX;
        }

        // Hitung berapa jarak yang sudah ditempuh tank dari titik start
        float jarakDitempuh = tankX - startX;

        // Ubah menjadi persentase 0.0f sampai 1.0f
        float hasilPersen = jarakDitempuh / totalLebarMap;

        // Batasi hasilnya agar tidak minus jika tank mundur lewat dari batas kiri
        hasilPersen = Mathf.Clamp01(hasilPersen);

        // Kirim data secara real-time ke UI
        uiProgressBar.UpdateLocalProgress(hasilPersen);
    }
}