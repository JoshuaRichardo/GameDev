using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTank : MonoBehaviour
{
    private InputMaster controls;
    private Vector2 moveInput;

    [Header("Movement Settings")]
    public float maxMoveSpeed = 5f;
    public float rotationSpeed = 100f;

    [Header("Acceleration Settings")]
    public float acceleration = 2f;
    public float deceleration = 3f;
    private float currentSpeed = 0f;

    private void Awake() => controls = new InputMaster();
    private void OnEnable() => controls.Player.Enable();
    private void OnDisable() => controls.Player.Disable();

    void Update()
    {
        moveInput = controls.Player.Move.ReadValue<Vector2>();

        HandleMovement();
    }

    void HandleMovement()
    {
        // 1. HITUNG AKSELERASI (W/S)
        float targetSpeed = moveInput.y * maxMoveSpeed;
        float lerpRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, lerpRate * Time.deltaTime);

        // 2. TERAPKAN MAJU/MUNDUR
        // Vector3.right karena sprite tank kamu menghadap kanan secara default
        transform.Translate(Vector3.right * currentSpeed * Time.deltaTime);

        // 3. HITUNG ROTASI (A/D)
        // Logika: Tank hanya berputar jika ada input belok (moveInput.x)
        if (Mathf.Abs(moveInput.x) > 0.1f)
        {
            // Jika sedang mundur (moveInput.y < 0), arah belok biasanya terbalik secara visual
            // Namun banyak game tank modern tetap menggunakan arah belok yang sama.
            // Di sini kita gunakan standar:
            float rotationDirection = -moveInput.x;

            // Jika mundur, kita balik arah rotasinya agar realistis (opsional)
            if (moveInput.y < 0) rotationDirection *= -1f;

            float rotationAmount = rotationDirection * rotationSpeed * Time.deltaTime;
            transform.Rotate(0, 0, rotationAmount);
        }
    }
}