using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemyPrefabs;
    public Transform player;

    public float spawnRadius = 20f;
    public int maxEnemies = 5; // Start with fewer enemies
    public float spawnInterval = 5f; // Time between spawns

    private float difficultyIncreaseInterval = 30f; // Increase difficulty every 30 sec
    private int maxEnemyIncreaseAmount = 2; // How many more enemies can be added
    private float minSpawnInterval = 1.5f; // Lowest possible spawn interval
    private float spawnIntervalDecrease = 0.5f; // How much the interval decreases

    private List<GameObject> activeEnemies = new List<GameObject>();

    void Start()
    {
        StartCoroutine(SpawnEnemies());
        StartCoroutine(IncreaseDifficultyOverTime());
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            activeEnemies.RemoveAll(enemy => enemy == null);

            if (activeEnemies.Count < maxEnemies)
            {
                SpawnEnemy();
            }
        }
    }

    IEnumerator IncreaseDifficultyOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(difficultyIncreaseInterval);

            maxEnemies += maxEnemyIncreaseAmount; // Increase max enemy count
            spawnInterval = Mathf.Max(spawnInterval - spawnIntervalDecrease, minSpawnInterval); // Reduce spawn interval but never below min

            Debug.Log($"Difficulty Increased! Max Enemies: {maxEnemies}, Spawn Interval: {spawnInterval}");
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefabs.Count == 0)
        {
            Debug.LogWarning("No enemy prefabs assigned to the spawner!");
            return;
        }

        Vector3 spawnPosition = GetRandomSpawnPosition();
        if (spawnPosition != Vector3.zero)
        {
            float terrainHeight = Terrain.activeTerrain.SampleHeight(spawnPosition);
            spawnPosition.y = terrainHeight + 1f;

            GameObject chosenEnemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
            GameObject newEnemy = Instantiate(chosenEnemyPrefab, spawnPosition, Quaternion.identity);
            activeEnemies.Add(newEnemy);

            Debug.Log("Spawned " + chosenEnemyPrefab.name + " at position: " + spawnPosition);
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * spawnRadius;
            randomDirection.y = 0f;
            Vector3 spawnPoint = player.position + randomDirection;

            if (NavMesh.SamplePosition(spawnPoint, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }
        return Vector3.zero;
    }
}
