using UnityEngine;

public class FollowHeadTipFull : MonoBehaviour
{
    [Header("Reference to the head tip")]
    public Transform headTip;    // Assign the empty GameObject

    [Header("Y Offset above head tip")]
    public float yOffset = 0.2f; // Vertical offset

    void LateUpdate()
    {
        if (headTip == null) return;

        // Follow the head tip completely
        Vector3 targetPosition = headTip.position;

        // Add fixed Y offset
        targetPosition.y += yOffset;

        transform.position = targetPosition;
    }
}
