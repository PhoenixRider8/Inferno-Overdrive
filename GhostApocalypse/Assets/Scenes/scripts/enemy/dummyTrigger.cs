using UnityEngine;

public class dummyTrigger : MonoBehaviour
{
    [Header("Enemy Health Reference")]
    [SerializeField] private EnemyHealthSprite eH;

    // Called manually by BulletScript when hit
    public void OnBulletHit(BulletScript bullet)
    {
        if (eH == null)
        {
            Debug.LogWarning($"[dummyTrigger] EnemyHealthSprite not assigned on {gameObject.name}!");
            return;
        }

        Debug.Log($"Dummy '{gameObject.name}' was hit by: {bullet.gameObject.name}");
        eH.TakeDamage(1);
    }

    // Optional physical fallback
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            var bulletScript = collision.gameObject.GetComponent<BulletScript>();
            if (bulletScript != null)
                OnBulletHit(bulletScript);
        }
    }
}
