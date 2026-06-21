using UnityEngine;

public class Checkpoint2 : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Mengecek apakah yang menyentuh area ini adalah tank pemain
        if (collision.CompareTag("Player"))
        {
            // Menyimpan status bahwa pemain sudah mencapai checkpoint
            PlayerPrefs.SetInt("HasCheckpoint", 1);
            
            // Menyimpan koordinat X dan Y dari menara ini
            PlayerPrefs.SetFloat("CheckpointX", transform.position.x);
            PlayerPrefs.SetFloat("CheckpointY", transform.position.y);
            
            Debug.Log("🔥 SISTEM: Checkpoint Level 2 Berhasil Tersimpan!");
        }
    }
}