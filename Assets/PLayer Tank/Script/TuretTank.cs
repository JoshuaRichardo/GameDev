using UnityEngine;
using UnityEngine.InputSystem;

public class Tiger2Turret : MonoBehaviour
{
    private InputMaster controls;
    private Vector2 mousePosition;

    [Header("Rotation Settings")]
    [Tooltip("Semakin tinggi, semakin cepat turret berputar mengejar mouse")]
    public float rotationSpeed = 5f;

    private void Awake()
    {
        controls = new InputMaster();
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    void Update()
    {
        // 1. Ambil posisi mouse dari Input System
        mousePosition = controls.Player.Look.ReadValue<Vector2>();

        // 2. Ubah posisi mouse (Screen Space) ke posisi dunia (World Space)
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePosition);

        // 3. Hitung arah dari turret ke posisi mouse
        Vector2 direction = new Vector2(
            worldMousePos.x - transform.position.x,
            worldMousePos.y - transform.position.y
        );

        // 4. Hitung sudut rotasi (Z-axis untuk 2D)
        // Kita gunakan Mathf.Atan2 untuk mendapatkan sudut dalam radian, lalu ubah ke derajat
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 5. Terapkan rotasi
        // Gunakan Quaternion.Lerp atau MoveTowards agar perputaran turret terasa berat (realistis)
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}