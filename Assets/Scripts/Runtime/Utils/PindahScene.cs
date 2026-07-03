using UnityEngine;
using UnityEngine.SceneManagement;

public class PindahScene : MonoBehaviour
{
    public GameData dataGame; // Tarik asset 'Pusat Data Game' ke sini di Inspector
    public string namaSceneTujuan; // Ketik nama scene tujuan (misal: "Level2")
    public int indeksMapTujuan; // Isi angka 1 untuk Level 2, angka 2 untuk Level 3, dst.

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Pastikan yang menabrak adalah Player
        if (collision.CompareTag("Player"))
        {
            if (dataGame != null)
            {
                // 1. Update index map ke level selanjutnya agar progress bar bertambah 25%
                dataGame.currentMapIndex = indeksMapTujuan;
                
                // 2. Amankan darah saat ini ke dalam GameData sebelum scene dihancurkan
                TankHealth playerHealth = collision.GetComponent<TankHealth>();
                if (playerHealth != null)
                {
                    dataGame.currentHealth = playerHealth.GetCurrentHealth();
                }
            }

            // 3. Pindah scene baru
            SceneManager.LoadScene(namaSceneTujuan);
        }
    }
}