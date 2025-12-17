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
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        if (moveX != 0 || moveZ != 0)
        {
            agent.ResetPath();
            Vector3 moveDir = new Vector3(moveX, 0, moveZ).normalized;
            agent.velocity = moveDir * moveSpeed;

            if (moveDir != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(moveDir);
            }
        }

        // --- LOGIKA 2: INPUT MOUSE (KLIK KANAN) ---
        // Ganti (0) jadi (1)
        else if (Input.GetMouseButton(1))
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Kita tambahkan LayerMask biar klik kanan cuma nempel di TANAH (bukan di musuh)
            // Ini biar kalau klik kanan musuh, dia gak jalan nembus musuh
            if (Physics.Raycast(ray, out hit))
            {
                agent.SetDestination(hit.point);
            }
        }
    }
}