using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavToPlayer : MonoBehaviour
{
    [Header("Navigation Settings")]
    public float stopDistance = 3f;
    public float attackCooldown = 3f; // seconds between attacks

    [Header("References")]
    public EnemyHealthSprite healthSprite;

    private NavMeshAgent agent;
    private Animator anim;
    private Transform player;
    private EnemyAnimationController animationController;

    private bool isAttacking = false;
    private bool canAttack = true;
    private Coroutine attackRoutine;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = transform.GetChild(0).GetComponent<Animator>();
        animationController = transform.GetChild(0).GetComponent<EnemyAnimationController>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (!player)
        {
            Debug.LogError($"[{name}] Player not found! Make sure Player has tag 'Player'.");
            enabled = false;
            return;
        }

        if (!healthSprite)
        {
            Debug.LogError($"[{name}] Missing EnemyHealthSprite reference!");
            enabled = false;
            return;
        }

        if (!animationController)
        {
            Debug.LogError($"[{name}] Missing EnemyAnimationController!");
            enabled = false;
            return;
        }
    }

    void Update()
    {
        if (!player || animationController.hasStartedDie) return; // stop everything if dead

        float distance = Vector3.Distance(transform.position, player.position);

        // Rotate to face player
        Vector3 dir = player.position - transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5f);

        // --- MOVE / ATTACK LOGIC ---
        if (isAttacking && distance > stopDistance)
        {
            // Player moved away → cancel attack immediately
            if (attackRoutine != null)
                StopCoroutine(attackRoutine);
            isAttacking = false;
            canAttack = false; // start cooldown
            StartCoroutine(Cooldown());
            SetMoveAnimation(true); // resume moving
        }

        if (!isAttacking && distance > stopDistance)
        {
            // chase
            agent.isStopped = false;
            agent.SetDestination(player.position);
            SetMoveAnimation(true);
        }
        else if (!isAttacking && distance <= stopDistance && canAttack)
        {
            // in attack range → start attack
            agent.isStopped = true;
            SetMoveAnimation(false);
            attackRoutine = StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        anim.ResetTrigger("attack");
        anim.SetTrigger("attack");

        // Wait until animation starts
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"));

        // Wait until ~90% of attack animation to deal damage
        yield return new WaitUntil(() =>
            anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f);

        // Deal damage if still in range
        if (player && Vector3.Distance(transform.position, player.position) <= stopDistance + 0.5f)
        {
            var health = player.GetComponent<playerHealth>();
            if (health)
            {
                health.takeDamage();
                Debug.Log($"[{name}] Player damaged! Health: {health.health}");
            }
        }

        // Wait until attack animation ends or player moves away
        yield return new WaitUntil(() =>
            !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") ||
            (player && Vector3.Distance(transform.position, player.position) > stopDistance));

        isAttacking = false;

        // Start cooldown
        canAttack = false;
        StartCoroutine(Cooldown());
    }

    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private void SetMoveAnimation(bool moving)
    {
        if (!anim || !anim.runtimeAnimatorController)
            return;

        int health = healthSprite.currentHealth;

        bool walk = health > 4; // green = walk
        bool crawl = health <= 4 && health > 0; // yellow = crawl

        anim.SetBool("isWalking", moving && walk);
        anim.SetBool("isYellow", moving && crawl);
        anim.SetBool("isDead", false);
    }
}
