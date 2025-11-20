using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target; // Seret object Player ke sini nanti

    [Header("Settings")]
    [Range(0.01f, 1f)]
    public float smoothSpeed = 0.125f; // Semakin kecil = Semakin delay/halus
    
    private Vector3 offset; // Jarak antara kamera dan player (disimpan otomatis)

    void Start()
    {
        // Hitung jarak posisi kamera sekarang ke player saat game mulai
        // Jadi kamu bebas atur posisi kamera di Scene View, script ini bakal ngikutin
        if (target != null)
        {
            offset = transform.position - target.position;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 1. Tentukan posisi tujuan kamera (Posisi Player + Jarak Awal)
        Vector3 desiredPosition = target.position + offset;

        // 2. Pindahkan kamera secara perlahan (Lerp) dari posisi sekarang ke tujuan
        // Ini yang bikin efek "Delay" itu
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // 3. Terapkan posisi baru
        transform.position = smoothedPosition;
    }
}