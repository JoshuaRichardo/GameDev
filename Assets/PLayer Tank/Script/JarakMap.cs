using UnityEngine;

public class jarakmap : MonoBehaviour
{
    [Header("Tarik Objek Penanda dari Hierarchy")]
    public Transform titikStartMap;   // Objek titik awal tank lahir di map ini
    
    [Header("Tarik Dua Pilihan Portal Akhir di Sini")]
    public Transform zonaPortal1;     // Tarik objek: Zona_PindahLevel
    public Transform zonaPortal2;     // Tarik objek: Zona_PindahLevel (1)

    private TankProgressBar uiProgressBar;
    private Transform portalTargetAktif;

    void Start()
    {
        CariProgressBar();
        PilihPortalTerdekat();
    }

    void CariProgressBar()
    {
        uiProgressBar = FindFirstObjectByType<TankProgressBar>();
    }

    // Fungsi otomatis memilih portal mana yang lebih dekat dengan jalur tank
    void PilihPortalTerdekat()
    {
        if (zonaPortal1 != null && zonaPortal2 == null)
        {
            portalTargetAktif = zonaPortal1;
        }
        else if (zonaPortal2 != null && zonaPortal1 == null)
        {
            portalTargetAktif = zonaPortal2;
        }
        else if (zonaPortal1 != null && zonaPortal2 != null)
        {
            // Hitung portal mana yang jarak X-nya paling dekat/masuk akal dari tank
            float jarakKe1 = Mathf.Abs(zonaPortal1.position.x - transform.position.x);
            float jarakKe2 = Mathf.Abs(zonaPortal2.position.x - transform.position.x);

            portalTargetAktif = (jarakKe1 < jarakKe2) ? zonaPortal1 : zonaPortal2;
        }
    }

    void Update()
    {
        if (uiProgressBar == null)
        {
            CariProgressBar();
            if (uiProgressBar == null) return;
        }

        // Jalankan pengecekan portal terdekat secara konstan saat tank bergerak
        PilihPortalTerdekat();

        if (titikStartMap == null || portalTargetAktif == null) return;

        float startX = titikStartMap.position.x;
        float portalX = portalTargetAktif.position.x;
        float tankX = transform.position.x;

        float totalLebarMap = portalX - startX;
        if (totalLebarMap <= 0f) return; // Mencegah pembagian dengan angka 0 atau minus

        // Hitung persentase berjalan dari 0.0 sampai 1.0 di dalam map ini
        float hasilPersen = (tankX - startX) / totalLebarMap;
        hasilPersen = Mathf.Clamp01(hasilPersen);

        // Kirim persentase pergerakan ini ke UI Progress Bar secara real-time
        uiProgressBar.UpdateLocalProgress(hasilPersen);
    }
}