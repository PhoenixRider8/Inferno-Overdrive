using System.Collections;
using UnityEngine;

public class MageAnimationController : MonoBehaviour
{
    [Header("References")]
    public EnemyHealthSprite healthSprite; // Reference to health tracker
    public Animator anim;
    public GameObject spellPrefab;
    public Transform spellSpawnPoint; // Hand position or spawn transform
    public GameObject player;

    [Header("Timing")]
    public float spellCooldown = 3f;

    private bool isAttacking = false;
    private bool isDead = false;
    private GameObject enemyRoot;

    void Start()
    {
        if (anim == null)
            anim = transform.GetChild(0).GetComponent<Animator>();

        enemyRoot = transform.root.gameObject;
        StartCoroutine(SpellRoutine());
    }

    IEnumerator SpellRoutine()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(spellCooldown);

            if (isDead) yield break; // stop if dead

            int health = healthSprite.currentHealth;

            // Trigger spell animation
            anim.SetTrigger("spell");
            isAttacking = true;

            yield return new WaitForSeconds(0.5f); // wait for hand raise timing

            if (spellPrefab && spellSpawnPoint)
            {
                if (health > 4)
                {
                    // Normal: single spell toward player
                    SpawnSpellAtPlayer();
                }
                else
                {
                    // Low health: 4-directional burst
                    SpawnDirectionalSpells();
                }
            }

            // wait for animation end before idle
            yield return new WaitForSeconds(1.5f);

            anim.SetTrigger("idle");
            isAttacking = false;
        }
    }

    void SpawnSpellAtPlayer()
    {
        Vector3 dir = (player.transform.position - spellSpawnPoint.position).normalized;
        GameObject spell = Instantiate(spellPrefab, spellSpawnPoint.position, Quaternion.LookRotation(dir));
        Rigidbody rb = spell.GetComponent<Rigidbody>();
        if (rb) rb.velocity = dir * 10f;
    }

    void SpawnDirectionalSpells()
    {
        // Fire N, S, E, W relative to player
        Vector3 playerPos = player.transform.position;
        Vector3[] directions = new Vector3[]
        {
            (playerPos + Vector3.forward - spellSpawnPoint.position).normalized,
            (playerPos + Vector3.back - spellSpawnPoint.position).normalized,
            (playerPos + Vector3.left - spellSpawnPoint.position).normalized,
            (playerPos + Vector3.right - spellSpawnPoint.position).normalized
        };

        foreach (var dir in directions)
        {
            GameObject spell = Instantiate(spellPrefab, spellSpawnPoint.position, Quaternion.LookRotation(dir));
            Rigidbody rb = spell.GetComponent<Rigidbody>();
            if (rb) rb.velocity = dir * 10f;
        }
    }

    void Update()
    {
        if (isDead) return;

        if (healthSprite.currentHealth <= 0)
        {
            isDead = true;
            StopAllCoroutines();
            anim.SetTrigger("die");
            StartCoroutine(HandleDeath());
        }
    }

    IEnumerator HandleDeath()
    {
        // Wait until about frame 56 (assuming 30 FPS)
        yield return new WaitForSeconds(56f / 30f);

        

        // Wait for the rest of the animation to finish
        yield return new WaitForSeconds(1.2f);
        PushBackOnDeath();
        // Add rigidbody constraints for post-death idle
        Rigidbody rb = enemyRoot.GetComponent<Rigidbody>();
        if (rb == null) rb = enemyRoot.AddComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation |
                         RigidbodyConstraints.FreezePositionX |
                         RigidbodyConstraints.FreezePositionZ;

        var detector = enemyRoot.AddComponent<MageGroundDetector>();
        detector.Setup(this);
    }


    public void OnHitGround()
    {
        
        Destroy(enemyRoot, 3f);
    }

    public void PushBackOnDeath()
    {
        // Direction opposite to where the mage is facing
        Vector3 pushDir = -transform.forward;

        // Apply impulse to root rigidbody
        Rigidbody rb = enemyRoot.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = enemyRoot.AddComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }

        float pushForce = 5f; // tweak this value
        rb.AddForce(pushDir * pushForce, ForceMode.Impulse);
    }

}
