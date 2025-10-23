using UnityEngine;
using System.Collections;

public class SimpleSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject prefabToSpawn;  // Object to spawn
    public Transform spawnPoint;      // Where to spawn (optional)
    public float spawnInterval = 3f;  // Time between spawns

    [Header("Target Settings")]
    public Transform target;          // The object to move toward
    public float moveSpeed = 3f;      // Speed of spawned objects

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnObject();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnObject()
    {
        if (prefabToSpawn == null || target == null) return;

        Vector3 spawnPos = spawnPoint ? spawnPoint.position : transform.position;
        GameObject obj = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

        // Add movement script dynamically
        MoveToTarget mover = obj.AddComponent<MoveToTarget>();
        mover.target = target;
        mover.moveSpeed = moveSpeed;
    }
}

public class MoveToTarget : MonoBehaviour
{
    public Transform target;
    public float moveSpeed = 3f;
    public float destroyDistance = 0.5f;  // Distance threshold to destroy object

    private void Update()
    {
        if (!target) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            moveSpeed * Time.deltaTime
        );

        // If close enough to target, destroy self
        if (Vector3.Distance(transform.position, target.position) <= destroyDistance)
        {
            Destroy(gameObject);
        }
    }
}
