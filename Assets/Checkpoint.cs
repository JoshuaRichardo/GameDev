using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Mengecek apakah yang masuk ke dalam radius adalah tank pemain
        if (collision.CompareTag("Player"))
        {
            // Menyimpan koordinat X dan Y dari Watch Tower ini ke memori permanen Unity
            PlayerPrefs.SetFloat("CheckpointX", transform.position.x);
            PlayerPrefs.SetFloat("CheckpointY", transform.position.y);
            
            // Memberi tanda (opsional) agar kita tahu checkpoint sudah tersimpan
            PlayerPrefs.SetInt("HasCheckpoint", 1);
            PlayerPrefs.Save();

            Debug.Log("Checkpoint Tersimpan di Watch Tower!");
        }
    }
}