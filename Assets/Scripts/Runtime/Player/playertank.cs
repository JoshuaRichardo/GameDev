using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTank : MonoBehaviour
{
    private InputMaster controls;
    private Vector2 moveInput;

    [Header("Movement Settings")]
    public float maxMoveSpeed = 1.8f;
    public float rotationSpeed = 60f;

    [Header("Acceleration Settings")]
    public float acceleration = 2f;
    public float deceleration = 3f;
    private float currentSpeed = 0f;

    [Header("Developer Tools (Testing)")]
    public bool hapusCheckpointSaatMulai = false; // <-- INI TOMBOL AJAIBNYA

    private void Awake() 
    {
        controls = new InputMaster();

        // 1. MENGHAPUS CHECKPOINT SEBELUM TANK MUNCUL
        if (hapusCheckpointSaatMulai == true)
        {
            PlayerPrefs.DeleteKey("HasCheckpoint");
            Debug.Log("DEV MODE: Checkpoint sukses dihapus paksa! Tank akan mulai dari awal.");
        }
    }

    private void OnEnable() => controls.Player.Enable();
    private void OnDisable() => controls.Player.Disable();

    void Start()
    {
        // Mengecek apakah pemain sudah pernah menyentuh checkpoint
        if (PlayerPrefs.GetInt("HasCheckpoint", 0) == 1)
        {
            float spawnX = PlayerPrefs.GetFloat("CheckpointX");
            float spawnY = PlayerPrefs.GetFloat("CheckpointY");

            transform.position = new Vector3(spawnX - 2f, spawnY, transform.position.z);
        }
    }

    void Update()
    {
        moveInput = controls.Player.Move.ReadValue<Vector2>();
        HandleMovement();

        if (Keyboard.current != null && Keyboard.current.qKey.wasPressedThisFrame)
        {
            // Menghapus memori checkpoint
            PlayerPrefs.DeleteKey("HasCheckpoint");
            
            // Memunculkan pesan di tab Console agar kamu tahu tombolnya berhasil dipencet
            Debug.Log("🔥 SISTEM: Memori Checkpoint berhasil dihapus! Jika game di-restart, tank akan mulai dari awal.");
        }
    }

    void HandleMovement()
    {
        float targetSpeed = moveInput.y * maxMoveSpeed;
        float lerpRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, lerpRate * Time.deltaTime);

        transform.Translate(Vector3.right * currentSpeed * Time.deltaTime);

        if (Mathf.Abs(moveInput.x) > 0.1f)
        {
            float rotationDirection = -moveInput.x;
            if (moveInput.y < 0) rotationDirection *= -1f;

            float rotationAmount = rotationDirection * rotationSpeed * Time.deltaTime;
            transform.Rotate(0, 0, rotationAmount);
        }
    }
}