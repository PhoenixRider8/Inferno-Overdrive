using UnityEngine;

public class EnemyHealthSprite : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 6;
    public int currentHealth;

    [Header("Sprite Settings")]
    public SpriteRenderer spriteRenderer;  // The floating health/sprite
    public Sprite greenSprite;
    public Sprite yellowSprite;
    public Sprite redSprite;

    [Header("Billboard & Positioning")]
    public Camera mainCamera;
    public Collider enemyCollider; // Assign the enemy's root collider
    public float yOffset = 0.2f;  // Offset above head

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;

        if (mainCamera == null)
            mainCamera = Camera.main;

        // Position the sprite above the enemy's head based on collider bounds
        UpdateSpritePosition();

        // Set initial color/sprite
        UpdateSprite();
    }

    void LateUpdate()
    {
        if (spriteRenderer == null || mainCamera == null) return;

        // Billboard effect: always face the camera
        spriteRenderer.transform.LookAt(
            spriteRenderer.transform.position + mainCamera.transform.rotation * Vector3.forward,
            mainCamera.transform.rotation * Vector3.up
        );
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);
        UpdateSprite();

        if (currentHealth <= 0)
        {
            isDead = true;
            // Do NOT call Die() here anymore
            // Animator will handle destruction after death animation
        }
    }


    private void UpdateSprite()
    {
        if (spriteRenderer == null) return;

        if (currentHealth > 4)
            spriteRenderer.sprite = greenSprite;
        else if (currentHealth > 2)
            spriteRenderer.sprite = yellowSprite;
        else
            spriteRenderer.sprite = redSprite;
    }

    private void UpdateSpritePosition()
    {
        if (spriteRenderer == null || enemyCollider == null) return;

        // Calculate top of enemy collider
        Vector3 top = enemyCollider.bounds.center + new Vector3(0, enemyCollider.bounds.extents.y + yOffset, 0);
        spriteRenderer.transform.position = top;
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
