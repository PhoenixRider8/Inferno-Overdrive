using UnityEngine;

public class SpellProjectile : MonoBehaviour
{
    public float lifetime = 5f;
    public float damage = 1f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var health = other.GetComponent<playerHealth>();
            if (health)
                health.takeDamage();

            Destroy(gameObject);
        }
        else if (other.CompareTag("Ground") || other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
