using UnityEngine;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    public Transform attackPoint;
    public LayerMask enemyLayers;
    public GameObject slashSpriteGameObject;

    private SpriteRenderer slashRenderer;

    // Kita simpan settingan awal yang sudah kamu atur di Inspector
    private Vector3 originalScale;
    private Quaternion originalRotation;

    [Header("Combat Settings")]
    public int baseDamage = 10;
    public float attackRange = 1.5f;
    public float attackCooldown = 0.5f;
    private float lastAttackTime = 0f;

    [Header("VFX Settings")]
    public float vfxDuration = 0.2f;
    // Scale Factor: Seberapa besar dia membesar? (1.5 artinya 1.5x lipat dari ukuran asli)
    public float scaleMultiplier = 1.5f;

    void Start()
    {
        if (slashSpriteGameObject != null)
        {
            slashRenderer = slashSpriteGameObject.GetComponent<SpriteRenderer>();

            // SIMPAN POSISI & ROTASI "SEMPURNA" YANG KAMU BUAT TADI
            originalScale = slashSpriteGameObject.transform.localScale;
            originalRotation = slashSpriteGameObject.transform.localRotation;

            slashSpriteGameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                StartCoroutine(AttackRoutine());
                lastAttackTime = Time.time;
            }
        }
    }

    IEnumerator AttackRoutine()
    {
        // 1. DAMAGE 
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider enemy in hitEnemies)
        {
            CharacterStats enemyStats = enemy.GetComponent<CharacterStats>();
            if (enemyStats != null) enemyStats.TakeDamage(baseDamage);
        }

        // 2. VISUAL EFEK
        if (slashSpriteGameObject != null && slashRenderer != null)
        {
            slashSpriteGameObject.SetActive(true);

            // RESET ke kondisi awal (sesuai settingan Inspector kamu)
            slashSpriteGameObject.transform.localScale = originalScale;
            slashSpriteGameObject.transform.localRotation = originalRotation; // PENTING: Balik ke rotasi setup awal

            // Reset Warna
            Color currentColor = slashRenderer.color;
            currentColor.a = 1f;
            slashRenderer.color = currentColor;

            Vector3 targetScale = originalScale * scaleMultiplier;

            float timer = 0f;
            while (timer < vfxDuration)
            {
                timer += Time.deltaTime;
                float t = timer / vfxDuration;

                // Animasi Membesar
                slashSpriteGameObject.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);

                // Animasi Menghilang (Fade Out)
                currentColor.a = Mathf.Lerp(1f, 0f, t);
                slashRenderer.color = currentColor;

                yield return null;
            }

            slashSpriteGameObject.SetActive(false);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}