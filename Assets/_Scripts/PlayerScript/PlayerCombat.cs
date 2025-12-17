using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    public Transform attackPoint; // Titik tengah ayunan pedang
    public LayerMask enemyLayers; // Biar kita cuma nyerang Musuh (bukan Teman/Tanah)
    private CharacterStats myStats; // Ambil stats diri sendiri
    public ParticleSystem slashEffect;

    [Header("Settings")]
    public float attackRange = 1.5f; // Jangkauan pedang
    public float attackCooldown = 0.5f; // Jeda antar serangan
    private float lastAttackTime = 0f;

    void Start()
    {
        myStats = GetComponent<CharacterStats>();
    }

    void Update()
    {
        // Ganti 'KeyCode.Space' jadi 'MouseButtonDown(0)' (Klik Kiri)
        // 0 = Kiri, 1 = Kanan, 2 = Tengah
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }
    }

    void Attack()
    {
        if (slashEffect != null)
        {
            slashEffect.Play();
        }
        // 1. Play Animation (Nanti diisi pas udah ada animasi)
        Debug.Log("Hiyyaaa! (Menghempaskan Pedang)");

        // 2. Deteksi musuh di dalam lingkaran area (Hitbox)
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        // 3. Proses Damage ke setiap musuh yang kena
        foreach (Collider enemy in hitEnemies)
        {
            // Ambil script stats si musuh
            CharacterStats enemyStats = enemy.GetComponent<CharacterStats>();

            if (enemyStats != null)
            {
                // HITUNG DAMAGE (ATK + CRIT)
                int finalDamage = CalculateDamage();

                // Kirim damage ke musuh
                enemyStats.TakeDamage(finalDamage);
            }
        }
    }

    // Rumus hitung output damage kita sendiri
    int CalculateDamage()
    {
        int damageToSend = myStats.baseAttack;

        // Cek Peluang Critical (Roll Dadu 0-100)
        float randomVal = Random.Range(0f, 100f);
        if (randomVal < myStats.critRate)
        {
            Debug.Log("CRITICAL HIT! 🔥");
            damageToSend = Mathf.RoundToInt(damageToSend * myStats.critDamageMultiplier);
        }

        return damageToSend;
    }

    // Fitur Debug: Biar kita bisa lihat lingkaran jangkauan pedang di Editor
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}