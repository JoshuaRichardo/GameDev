using UnityEngine;
using UnityEngine.InputSystem;

public class Tiger2Turret : MonoBehaviour
{
    private InputMaster controls;
    private Vector3 mousePosition;
    private Vector3 worldMousePos;
    [Header("Rotation Settings")]
    [Tooltip("Semakin tinggi, semakin cepat turret berputar mengejar mouse")]
    public float rotationSpeed = 5f;




    void Update()
    {


        // 1. Ambil posisi mouse dari Input System
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3 direction = mousePosition - transform.position;

        // 4. Hitung sudut rotasi (Z-axis untuk 2D)
        // Kita gunakan Mathf.Atan2 untuk mendapatkan sudut dalam radian, lalu ubah ke derajat
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 5. Terapkan rotasi
        // Gunakan Quaternion.Lerp atau MoveTowards agar perputaran turret terasa berat (realistis)
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}