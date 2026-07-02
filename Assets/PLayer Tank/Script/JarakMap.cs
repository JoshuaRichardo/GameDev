using UnityEngine;

public class jarakmap : MonoBehaviour
{
    [Header("Tarik Objek Penanda dari Hierarchy")]
    public Transform titikStartMap;   // Objek titik awal tank lahir di map ini
    public Transform titikPortalMap;  // Tarik objek Zona_PindahLevel milik map ini!

    private TankProgressBar uiProgressBar;

    void Start()
    {
        CariProgressBar();
    }

    void CariProgressBar()
    {
        uiProgressBar = FindFirstObjectByType<TankProgressBar>();
    }

    void Update()
    {
        if (uiProgressBar == null)
        {
            CariProgressBar();
            if (uiProgressBar == null) return;
        }

        if (titikStartMap == null || titikPortalMap == null) return;

        float startX = titikStartMap.position.x;
        float portalX = titikPortalMap.position.x;
        float tankX = transform.position.x;

        float totalLebarMap = portalX - startX;
        if (totalLebarMap == 0f) return;

        // Hitung persentase berjalan dari 0.0 sampai 1.0 di dalam map ini
        float hasilPersen = (tankX - startX) / totalLebarMap;
        hasilPersen = Mathf.Clamp01(hasilPersen);

        // Kirim persentase pergerakan ini ke UI Progress Bar secara real-time
        uiProgressBar.UpdateLocalProgress(hasilPersen);
    }
}