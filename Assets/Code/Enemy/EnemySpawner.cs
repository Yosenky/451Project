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

    private float statMultiplier = 1f;
    private float statMultiplierIncrease = 0.25f;

    private List<GameObject> activeEnemies = new List<GameObject>();

    public GameObject meleeAuraPrefab;
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
            statMultiplier += statMultiplierIncrease;

            //Debug.Log($"Difficulty Increased! Max Enemies: {maxEnemies}, Spawn Interval: {spawnInterval}");
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
            ScaleEnemyStats(newEnemy);

            //Debug.Log("Spawned " + chosenEnemyPrefab.name + " at position: " + spawnPosition);
        }


    }

    void ScaleEnemyStats(GameObject enemy)
    {
       

        
        if (enemy.TryGetComponent<RangedLangsat>(out RangedLangsat ranged))
        {
           
            ranged.health *= statMultiplier;
            ranged.attackdamage = Mathf.RoundToInt(ranged.attackdamage * statMultiplier);
            ranged.agent.speed *= statMultiplier;
            ranged.projectileSpeed *= statMultiplier;
            ranged.attackRange *= statMultiplier;
            ActivateDifficultyAttachments(enemy, statMultiplier);
        }
        else if (enemy.TryGetComponent<EnemyAI>(out EnemyAI melee)) 
        {
            melee.health *= statMultiplier;
            melee.attackdamage = Mathf.RoundToInt(melee.attackdamage * statMultiplier);
            melee.agent.speed *= statMultiplier;
            if (meleeAuraPrefab != null)
            {
                GameObject aura = Instantiate(meleeAuraPrefab, enemy.transform);
                aura.transform.localPosition = Vector3.zero;

                ParticleSystem ps = aura.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    var main = ps.main;
                    main.startColor = GetColorByDifficulty(statMultiplier);
                }
            }
        }
        else if (enemy.TryGetComponent<Rollingenemy>(out Rollingenemy explodey)) 
        {
            
            explodey.health *= statMultiplier;
            explodey.explosionDamage = Mathf.RoundToInt(explodey.explosionDamage * statMultiplier);
            ActivateDifficultyAttachments(enemy, statMultiplier);
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
    Color GetColorByDifficulty(float multiplier)
    {
        if (multiplier < 1.5f)
            return Color.green;
        else if (multiplier < 2f)
            return Color.yellow;
        else if (multiplier < 3f)
            return new Color(1f, 0.5f, 0f); 
        else
            return Color.red;
    }

    void ActivateDifficultyAttachments(GameObject enemy, float difficulty)
    {
        Transform easyAttachment = null;
        Transform mediumAttachment = null;
        Transform hardAttachment = null;

        // Iterate over child objects and assign them to difficulty tiers
        foreach (Transform child in enemy.transform)
        {
            if (child.CompareTag("DifficultyEasy"))
                easyAttachment = child;
            else if (child.CompareTag("DifficultyMedium"))
                mediumAttachment = child;
            else if (child.CompareTag("DifficultyHard"))
                hardAttachment = child;
        }

        // Disable all first
        if (easyAttachment) easyAttachment.gameObject.SetActive(false);
        if (mediumAttachment) mediumAttachment.gameObject.SetActive(false);
        if (hardAttachment) hardAttachment.gameObject.SetActive(false);

        // Enable the appropriate attachment
        if (difficulty < 1.5f && easyAttachment)
            easyAttachment.gameObject.SetActive(true);
        else if (difficulty < 2.5f && mediumAttachment)
            mediumAttachment.gameObject.SetActive(true);
        else if (hardAttachment)
            hardAttachment.gameObject.SetActive(true);
    }
}
