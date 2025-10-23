using UnityEngine;

public class dummyTrigger : MonoBehaviour
{
    // This gets called manually by BulletScript when hit
    public void OnBulletHit(BulletScript bullet)
    {
        Debug.Log($"Dummy was hit by: {bullet.gameObject.tag}");
        // Example: TakeDamage(10);
    }

    // If bullet uses physical collision, this also works automatically
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Collision detected: {collision.gameObject.tag}");

        // If it’s a bullet, trigger logic manually
        if (collision.gameObject.CompareTag("Bullet"))
        {
            var bulletScript = collision.gameObject.GetComponent<BulletScript>();
            if (bulletScript != null)
                OnBulletHit(bulletScript);
        }
    }
}
