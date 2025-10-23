using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticlePrefabSpawner : MonoBehaviour
{
    public GameObject prefabToSpawn; // Prefab to attach to each particle
    private ParticleSystem ps;
    private ParticleSystem.Particle[] particles;
    private GameObject[] spawnedPrefabs;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[ps.main.maxParticles];
        spawnedPrefabs = new GameObject[ps.main.maxParticles];
    }

    void LateUpdate()
    {
        int aliveCount = ps.GetParticles(particles);

        for (int i = 0; i < aliveCount; i++)
        {
            if (spawnedPrefabs[i] == null)
            {
                spawnedPrefabs[i] = Instantiate(prefabToSpawn, transform);
            }

            // Match prefab position to particle position
            spawnedPrefabs[i].transform.position = particles[i].position + transform.position;
        }

        // Disable unused prefabs if particles die
        for (int i = aliveCount; i < spawnedPrefabs.Length; i++)
        {
            if (spawnedPrefabs[i] != null)
            {
                Destroy(spawnedPrefabs[i]);
                spawnedPrefabs[i] = null;
            }
        }
    }
}
