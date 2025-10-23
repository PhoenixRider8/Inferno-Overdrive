using UnityEngine;

public class enemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 6;
    private int currentHealth;

    [Header("Material Reference")]
    public Renderer enemyRenderer; // Assign the enemy's Renderer (MeshRenderer/SkinnedMeshRenderer)
    private Material enemyMat;     // Material instance

    [Header("Death Settings")]
    public Animator animator;      // Assign Animator (optional)
    public string deathTrigger = "Die"; // Trigger name in Animator
    public float destroyDelay = 2f;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;

        // Create material instance so color change doesn't affect others
        if (enemyRenderer != null)
            enemyMat = enemyRenderer.material;

        if (enemyMat != null)
            enemyMat.color = Color.green;
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);

        UpdateColor();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateColor()
    {
        if (enemyMat == null) return;

        if (currentHealth > 4)
            enemyMat.color = Color.green;
        else if (currentHealth > 2)
            enemyMat.color = Color.yellow;
        else
            enemyMat.color = Color.red;
    }

    void Die()
    {
        isDead = true;

        if (animator != null)
            animator.SetTrigger(deathTrigger);

        // Optionally disable collider or movement script
        Collider col = GetComponent<Collider>();
        if (col) col.enabled = false;

        Destroy(gameObject, destroyDelay);
    }
}
