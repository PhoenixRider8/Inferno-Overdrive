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

    // ✅ ensures one hit per bullet
    private bool hasHit = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, lifetime);

        if (!useRaycast && rb == null)
            Debug.LogWarning($"[BulletScript] '{name}' is set to physical mode but has no Rigidbody!");

        if (useRaycast && rb != null)
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }
    }

    void Update()
    {
        if (!useRaycast || hasHit)
            return;

        // 🔫 Instant Raycast bullet logic
        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance, ~ignoreLayer))
        {
            HandleHit(hit.collider.gameObject, hit.point, hit.normal);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (useRaycast || hasHit)
            return;

        HandleHit(collision.gameObject, collision.contacts[0].point, collision.contacts[0].normal);
    }

    private void HandleHit(GameObject target, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (hasHit) return;
        hasHit = true;

        gameObject.tag = "Bullet";

        // --- Wall Hit ---
        if (target.CompareTag("LevelPart"))
        {
            if (decalHitWall)
                Instantiate(decalHitWall, hitPoint + hitNormal * floatInfrontOfWall, Quaternion.LookRotation(hitNormal));
        }

        // --- Dummy Hit ---
        else if (target.CompareTag("Dummie"))
        {
            if (bloodEffect)
                Instantiate(bloodEffect, hitPoint, Quaternion.LookRotation(hitNormal));

            // ✅ get parent trigger to avoid double hits from child colliders
            var dummy = target.GetComponentInParent<dummyTrigger>();
            if (dummy != null)
                dummy.OnBulletHit(this);
        }

        // Small delay to ensure effects spawn before destroy
        Destroy(gameObject, 0.02f);
    }
}
