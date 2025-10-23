using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyAnimationController : MonoBehaviour
{
    [Header("References")]
    public EnemyHealthSprite healthSprite; // Assign EnemyHealthSprite
    public Animator animator;
    public bool hasStartedDie = false;

    // Root object to destroy
    private GameObject enemyRoot;

    void Start()
    {
        if (healthSprite == null)
        {
            Debug.LogError($"[{name}] Missing EnemyHealthSprite!");
            enabled = false;
            return;
        }

        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError($"[{name}] Missing Animator!");
            enabled = false;
            return;
        }

        // Determine root enemy object to destroy
        enemyRoot = transform.root.gameObject;
    }

    void Update()
    {
        int health = healthSprite.currentHealth;

        // Green → Walk
        if (health > 4)
        {
            animator.SetBool("isYellow", false);
            animator.SetBool("isDead", false);
        }
        // Yellow → Crawl
        else if (health > 0)
        {
            animator.SetBool("isYellow", true);
            animator.SetBool("isDead", false);
        }
        // Red / Dead → Die
        else if (health <= 0 && !hasStartedDie)
        {
            hasStartedDie = true;
            animator.SetBool("isDead", true);

            // Wait for Die animation to finish, then destroy root
            StartCoroutine(DestroyAfterDieAnimation());
        }
    }

    public IEnumerator DestroyAfterDieAnimation()
    {
        // Wait until Animator is actually in Die state (handles transitions)
        bool dieStateActive = false;
        while (!dieStateActive)
        {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            if (state.IsName("Die") && state.normalizedTime >= 0f)
                dieStateActive = true;
            else
                yield return null;
        }

        // Wait for the actual length of the Die clip
        float clipLength = 0f;
        AnimatorClipInfo[] clips = animator.GetCurrentAnimatorClipInfo(0);
        foreach (var clip in clips)
        {
            if (clip.clip.name.ToLower().Contains("die"))
            {
                clipLength = clip.clip.length;
                break;
            }
        }

        // Wait for Die animation to finish
        yield return new WaitForSeconds(clipLength);

        // Destroy the root enemy object
        Destroy(enemyRoot);
    }

    public void die()
    {
        Destroy(enemyRoot);

    }
}
