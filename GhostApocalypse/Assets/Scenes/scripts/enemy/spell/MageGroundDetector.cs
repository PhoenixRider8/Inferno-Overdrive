using UnityEngine;

public class MageGroundDetector : MonoBehaviour
{
    private MageAnimationController controller;

    public void Setup(MageAnimationController ctrl)
    {
        controller = ctrl;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            controller.OnHitGround();
        }
    }
}
