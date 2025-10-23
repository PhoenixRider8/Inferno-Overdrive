using System.Collections.Generic;
using UnityEngine;

public class StageProgressManager : MonoBehaviour
{
    [Header("Enemy References")]
    public List<GameObject> enemies = new List<GameObject>();  // Assign all enemies in this stage

    [Header("UI Reference")]
    public PlayerHealthBarDual playerUI;  // Reference to PlayerHealthBarDual (for showing Win screen)

    private bool stageCleared = false;
    private bool enemiesActivated = false; // Track if enemies have ever been activated

    void Update()
    {
        if (stageCleared) return;

        // Clean up null references (destroyed enemies)
        enemies.RemoveAll(e => e == null);

        // Check if any enemy is active at least once
        foreach (var enemy in enemies)
        {
            if (enemy != null && enemy.activeInHierarchy)
            {
                enemiesActivated = true;
                break;
            }
        }

        // Only check for win if enemies were activated
        if (enemiesActivated)
        {
            bool allDefeated = true;
            foreach (var enemy in enemies)
            {
                if (enemy != null && enemy.activeInHierarchy)
                {
                    allDefeated = false;
                    break;
                }
            }

            if (allDefeated)
            {
                stageCleared = true;
                Debug.Log("🎉 Stage Cleared! Player Wins!");

                if (playerUI)
                    playerUI.ShowResult(true);
            }
        }
    }
}
