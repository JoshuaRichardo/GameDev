using UnityEngine;
using UnityEngine.EventSystems;

public class HandCrankUI : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    [Header("Settings")]
    public RectTransform crankHandle; // Gambar gagang yang akan berputar
    public float sensitivity = 0.5f;   // Sensitivitas putaran
    public float smoothing = 10f;    // Kehalusan gerakan visual

    [Header("Output")]
    public float totalRotationDelta; // Total derajat yang sudah "ditabung"

    private Vector2 lastMousePos;
    private float visualAngle;

    public void OnPointerDown(PointerEventData eventData)
    {
        lastMousePos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Hitung selisih gerakan mouse horizontal (X)
        float deltaX = eventData.position.x - lastMousePos.x;

        // Tambahkan ke tabungan rotasi
        totalRotationDelta += deltaX * sensitivity;

        lastMousePos = eventData.position;
    }

    void Update()
    {
        // Putar visual gagang secara halus mengikuti totalRotationDelta
        visualAngle = Mathf.LerpAngle(visualAngle, totalRotationDelta, Time.deltaTime * smoothing);
        crankHandle.localRotation = Quaternion.Euler(0, 0, -visualAngle);
    }
}