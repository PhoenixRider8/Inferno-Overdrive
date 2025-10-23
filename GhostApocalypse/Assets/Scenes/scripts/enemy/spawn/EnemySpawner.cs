using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;              // Enemy prefab to spawn
    public Transform[] spawnPoints;             // Possible spawn locations
    public float timeBetweenWaves = 5f;         // Delay between waves

    private int[] waveCounts = new int[] { 3, 6, 9 }; // Number of enemies per wave

    void Start()
    {
        if (enemyPrefab == null || spawnPoints.Length == 0)
        {
            Debug.LogError("EnemySpawner: Missing prefab or spawn points!");
            return;
        }

        StartCoroutine(SpawnWaves());
    }

    private IEnumerator SpawnWaves()
    {
        for (int waveIndex = 0; waveIndex < waveCounts.Length; waveIndex++)
        {
            int enemiesToSpawn = waveCounts[waveIndex];
            Debug.Log($"Spawning wave {waveIndex + 1}: {enemiesToSpawn} enemies");

            for (int i = 0; i < enemiesToSpawn; i++)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            }

            // Wait before next wave
            yield return new WaitForSeconds(timeBetweenWaves);
        }

        Debug.Log("All waves spawned!");
    }
}
