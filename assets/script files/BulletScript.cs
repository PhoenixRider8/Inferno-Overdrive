using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [Header("General Settings")]
    [Tooltip("Choose between instant raycast hit or physical collision bullet.")]
    public bool useRaycast = true;

    [Tooltip("Maximum raycast distance (for instant hit mode).")]
    public float maxDistance = 1000f;

    [Tooltip("How long before the bullet is destroyed automatically.")]
    public float lifetime = 2f;

    [Header("Effects & Prefabs")]
    [Tooltip("Prefab for wall impact decal.")]
    public GameObject decalHitWall;

    [Tooltip("Offset for decal to prevent z-fighting.")]
    public float floatInfrontOfWall = 0.05f;

    [Tooltip("Prefab for blood particle effect.")]
    public GameObject bloodEffect;

    [Tooltip("Layers to ignore (e.g., Player, Weapon).")]
    public LayerMask ignoreLayer;

    private RaycastHit hit;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Automatically destroy the bullet after its lifetime
        Destroy(gameObject, lifetime);

        // If using physical mode but missing Rigidbody, warn developer
        if (!useRaycast && rb == null)
            Debug.LogWarning($"[BulletScript] Bullet '{name}' is set to physical mode but has no Rigidbody!");

        // If using raycast mode, disable physics for safety
        if (useRaycast && rb != null)
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }
    }

    void Update()
    {
        if (!useRaycast)
            return; // skip raycast logic if using physical bullet

        // 🔫 Instant Raycast bullet logic
        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance, ~ignoreLayer))
        {
            HandleHit(hit.collider.gameObject, hit.point, hit.normal);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (useRaycast)
            return; // skip collision if using hitscan mode

        // 🚀 Physical collision bullet logic
        HandleHit(collision.gameObject, collision.contacts[0].point, collision.contacts[0].normal);
    }

    private void HandleHit(GameObject target, Vector3 hitPoint, Vector3 hitNormal)
    {
        // ✅ Change tag when bullet hits something
        gameObject.tag = "Bullet";
        Debug.Log($"Bullet hit: {target.name} | Bullet tag now: {gameObject.tag}");

        // --- Handle Wall Hit ---
        if (target.CompareTag("LevelPart"))
        {
            if (decalHitWall)
                Instantiate(decalHitWall, hitPoint + hitNormal * floatInfrontOfWall, Quaternion.LookRotation(hitNormal));
        }

        // --- Handle Dummy Hit ---
        else if (target.CompareTag("Dummie"))
        {
            if (bloodEffect)
                Instantiate(bloodEffect, hitPoint, Quaternion.LookRotation(hitNormal));

            // ✅ Tell dummyTrigger it was hit
            var dummy = target.GetComponent<dummyTrigger>();
            if (dummy != null)
                dummy.OnBulletHit(this);
        }

        // Small delay before destruction so tag and logs register
        Destroy(gameObject, 0.05f);
    }
}
