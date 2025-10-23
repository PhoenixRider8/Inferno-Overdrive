using System.Collections.Generic;
using UnityEngine;

public class ActivateEnemiesOnTrigger : MonoBehaviour
{
    [Header("Enemies to Activate")]
    public List<GameObject> enemies = new List<GameObject>();

    private bool hasActivated = false;


    private void Start()
    {
        // Deactivate all enemies at start
        foreach (var enemy in enemies)
        {
            if (enemy != null)
                enemy.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasActivated) return;

        if (other.CompareTag("Player"))
        {
            foreach (var enemy in enemies)
            {
                if (enemy != null)
                    enemy.SetActive(true);
            }

            hasActivated = true;

            // Just disable trigger collider so it doesn't re-fire
            var col = GetComponent<Collider>();
            if (col) col.enabled = false;
        }
    }
}
