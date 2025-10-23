using UnityEngine;

public class MageFacePlayer : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotateSpeed = 5f; // smooth rotation speed

    [Header("References")]
    public Transform player; // auto-assigned if null
    public Animator anim; // reference to mage’s animator

    private bool canRotate = true;

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (anim == null)
            anim = GetComponentInChildren<Animator>();

        if (player == null)
        {
            Debug.LogError($"[{name}] Player not found! Make sure Player has tag 'Player'.");
            enabled = false;
        }
    }

    void Update()
    {
        if (!canRotate || player == null) return;

        // don’t rotate while casting spell
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
        if (state.IsName("spell")) return;

        // smoothly rotate to face player
        Vector3 dir = player.position - transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude > 0.01f)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * rotateSpeed);
        }
    }

    // called from animation events or mage controller
    public void EnableRotation(bool enable)
    {
        canRotate = enable;
    }
}
