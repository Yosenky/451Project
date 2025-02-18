using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{

    public GameObject enemyPrefab;
    public Transform player;

    public float spawnRadius = 20f;
    public int maxEnemies = 10;
    public float spawnInterval = 5f;
    private int currentEnemyCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            if(currentEnemyCount < maxEnemies)
            {
                SpawnEnemy();
            }
        }
    }

    void SpawnEnemy()
    {
        Vector3 spawnPosition = GetRandomSpawnPosition();
        if(spawnPosition != Vector3.zero)
        {
            // our new terrain height is around 15. we need to take that into account
            float terrainHeight = Terrain.activeTerrain.SampleHeight(spawnPosition);
            spawnPosition.y = terrainHeight + 1f;

            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            currentEnemyCount++;
        }
    }


    Vector3 GetRandomSpawnPosition()
    {
        for (int i=0; i < 10; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * spawnRadius;
            randomDirection.y = 0f;
            Vector3 spawnPoint = player.position + randomDirection;

            NavMeshHit hit;
            if(NavMesh.SamplePosition(spawnPoint, out hit, 5f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }
        return Vector3.zero;
    }

    
}
