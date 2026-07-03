using UnityEngine;

public class PanahPenunjuk : MonoBehaviour
{
    [Header("Masukkan Target Tujuan ke Sini:")]
    public Transform targetTujuan; 

    void Update()
    {
        if (targetTujuan != null)
        {
            Vector2 arah = targetTujuan.position - transform.position;
            
            // Menggunakan "right" karena gambar aslimu menghadap kanan
            transform.right = arah; 
        }
    }
}