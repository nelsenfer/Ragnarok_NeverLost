using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("Base Stats")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Offensive Stats")]
    public int baseAttack = 10;
    [Range(0f, 100f)] public float critRate = 10f; // 10% Chance
    public float critDamageMultiplier = 2.0f; // 2x Lipat kalau crit

    [Header("Defensive Stats")]
    public int physicalDefense = 2;

    void Start()
    {
        currentHealth = maxHealth;
    }

    // Fungsi untuk Menerima Damage (Dipanggil oleh penyerang)
    public void TakeDamage(int damage)
    {
        // 1. Rumus Defense: Damage dikurangi Defense dulu
        int damageAfterDef = damage - physicalDefense;

        // Pastikan damage minimal 1, jangan sampai minus (malah nambah darah)
        if (damageAfterDef < 1) damageAfterDef = 1;

        // 2. Kurangi HP
        currentHealth -= damageAfterDef;
        Debug.Log(transform.name + " kena " + damageAfterDef + " damage! Sisa HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(transform.name + " Meninggoyyy... 💀");
        // Nanti bisa tambah animasi mati atau drop item di sini
        Destroy(gameObject);
    }
}