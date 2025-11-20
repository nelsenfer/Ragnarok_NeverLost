using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private Camera mainCam;

    [Header("Settings")]
    public float moveSpeed = 6f; // Kecepatan jalan (bisa diatur di Inspector)

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        mainCam = Camera.main;

        // Samain kecepatan Agent dengan settingan kita
        agent.speed = moveSpeed;
        agent.acceleration = 999f; // Biar responsif, gak pake ngerem lama
        agent.angularSpeed = 999f; // Biar muternya cepet
    }

    void Update()
    {
        // --- LOGIKA 1: INPUT KEYBOARD (WASD) ---
        float moveX = Input.GetAxisRaw("Horizontal"); // A / D
        float moveZ = Input.GetAxisRaw("Vertical");   // W / S

        // Kalau ada tombol WASD yang ditekan...
        if (moveX != 0 || moveZ != 0)
        {
            // 1. PENTING: Reset path/tujuan mouse sebelumnya biar gak berebut
            agent.ResetPath();

            // 2. Hitung arah gerak
            Vector3 moveDir = new Vector3(moveX, 0, moveZ).normalized;

            // 3. Gerakkan Agent pakai Velocity (bukan Transform) biar tetep patuh sama NavMesh
            // (Jadi gak bakal nembus tembok)
            agent.velocity = moveDir * moveSpeed;

            // 4. Putar badan karakter sesuai arah jalan
            if (moveDir != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(moveDir);
            }
        }

        // --- LOGIKA 2: INPUT MOUSE (KLIK) ---
        // Cuma bisa klik kalau WASD lagi gak dipencet (biar gak bingung)
        else if (Input.GetMouseButton(0))
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                agent.SetDestination(hit.point);
            }
        }
    }
}