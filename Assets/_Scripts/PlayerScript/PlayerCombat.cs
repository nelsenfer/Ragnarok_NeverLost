using UnityEngine;
using UnityEngine.AI; // WAJIB ADA: Biar bisa akses NavMeshAgent
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    public Transform attackPoint;
    public LayerMask enemyLayers;
    public GameObject slashSpriteGameObject;

    private SpriteRenderer slashRenderer;
    private NavMeshAgent agent; // KITA BUTUH KAKINYA PLAYER
    private Vector3 originalScale;
    private Quaternion originalRotation;

    [Header("Combat Settings")]
    public int baseDamage = 10;
    public float attackRange = 1.5f;
    public float attackCooldown = 0.5f;
    private float lastAttackTime = 0f;

    [Header("Auto-Target Settings")]
    public float targetingRange = 10f; // Jarak deteksi diperjauh biar bisa ngejar dari jauh

    [Header("VFX Settings")]
    public float vfxDuration = 0.2f;
    public float scaleMultiplier = 1.5f;

    // Variable buat nyimpen musuh yang lagi dikejar
    private Transform currentTarget;
    private bool isChasing = false;

    void Start()
    {
        // Ambil komponen NavMeshAgent dari player sendiri
        agent = GetComponent<NavMeshAgent>();

        if (slashSpriteGameObject != null)
        {
            slashRenderer = slashSpriteGameObject.GetComponent<SpriteRenderer>();
            originalScale = slashSpriteGameObject.transform.localScale;
            originalRotation = slashSpriteGameObject.transform.localRotation;
            slashSpriteGameObject.SetActive(false);
        }
    }

    void Update()
    {
        // KLIK KIRI (Serang)
        if (Input.GetMouseButtonDown(0))
        {
            // 1. Cari musuh terdekat
            Transform targetEnemy = GetNearestEnemy();

            if (targetEnemy != null)
            {
                // Kalau ada musuh, mulai proses pengejaran
                // Stop pengejaran lama kalau ada, ganti target baru
                StopAllCoroutines();
                StartCoroutine(ChaseAndAttackRoutine(targetEnemy));
            }
            else
            {
                // Kalau gak ada musuh di sekitar, serang di tempat aja (hit angin)
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    PerformAttackVFX(); // Cuma animasi tanpa damage
                    lastAttackTime = Time.time;
                }
            }
        }
    }

    // --- LOGIKA UTAMA: KEJAR DULU BARU PUKUL ---
    IEnumerator ChaseAndAttackRoutine(Transform target)
    {
        isChasing = true;
        currentTarget = target;

        // Loop: Selama jarak kita ke musuh MASIH JAUH (> attackRange)
        // Kita suruh player lari deketin
        while (Vector3.Distance(transform.position, target.position) > attackRange)
        {
            // Cek: Kalau musuhnya mati/ilang pas dikejar, stop.
            if (target == null) yield break;

            agent.SetDestination(target.position); // Lari ke musuh
            yield return null; // Tunggu frame berikutnya, cek lagi
        }

        // --- BAGIAN INI JALAN PAS UDAH DEKET (SAMPAI) ---

        isChasing = false;
        agent.ResetPath(); // REM MENDADAK (Stop jalan)

        // Hadap musuh biar pas
        FaceTarget(target);

        // Cek cooldown serang
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            PerformAttackLogic(); // Pukul Beneran (Damage)
            PerformAttackVFX();   // Keluarin Efek
            lastAttackTime = Time.time;
        }
    }

    // Fungsi Cari Musuh (Sama kayak sebelumnya tapi return Transform)
    Transform GetNearestEnemy()
    {
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, targetingRange, enemyLayers);
        Collider nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider enemy in enemiesInRange)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = enemy;
            }
        }

        if (nearest != null) return nearest.transform;
        return null;
    }

    void FaceTarget(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero) transform.rotation = Quaternion.LookRotation(direction);
    }

    // Logic Ngurangin Darah
    void PerformAttackLogic()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider enemy in hitEnemies)
        {
            CharacterStats enemyStats = enemy.GetComponent<CharacterStats>();
            if (enemyStats != null) enemyStats.TakeDamage(baseDamage);
        }
    }

    // Logic Visual Efek (Animasi Tebasan)
    void PerformAttackVFX()
    {
        if (slashSpriteGameObject != null && slashRenderer != null)
        {
            StartCoroutine(VFXRoutine());
        }
    }

    IEnumerator VFXRoutine()
    {
        slashSpriteGameObject.SetActive(true);

        // Reset Posisi & Rotasi
        slashSpriteGameObject.transform.localScale = originalScale;
        slashSpriteGameObject.transform.localRotation = originalRotation;

        Color col = slashRenderer.color;
        col.a = 1f;
        slashRenderer.color = col;

        Vector3 targetScale = originalScale * scaleMultiplier;
        float timer = 0f;
        while (timer < vfxDuration)
        {
            timer += Time.deltaTime;
            float t = timer / vfxDuration;
            slashSpriteGameObject.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            col.a = Mathf.Lerp(1f, 0f, t);
            slashRenderer.color = col;
            yield return null;
        }
        slashSpriteGameObject.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint != null) { Gizmos.color = Color.red; Gizmos.DrawWireSphere(attackPoint.position, attackRange); }
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, targetingRange);
    }
}