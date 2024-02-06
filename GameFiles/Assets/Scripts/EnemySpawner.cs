using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float xRange = 4.0f; // The X range for spawning enemies
    public float spawnRate = 1.5f; // How often enemies spawn
    public float enemySpeed = 20.0f; // Speed at which the enemy moves towards the player
    public float despawnZ = -4.0f; // Z position at which enemies despawn
    private float nextSpawnTime;
    private int enemiesSpawned = 0;
    public GameObject[] powerUpPrefabs;
    public int powerUpWaves = 4;

    void SpawnPowerUps()
    {
        int numberOfPowerUps = powerUpPrefabs.Length;
        float[] desiredPositions = { 1, 5, 9 };

        List<int> powerUpIndices = new List<int>();
        for (int i = 0; i < numberOfPowerUps; i++)
        {
            powerUpIndices.Add(i);
        }

        ShuffleList(powerUpIndices);

        for (int i = 0; i < numberOfPowerUps; i++)
        {
            int powerUpIndex = powerUpIndices[i];
            float powerUpX = -xRange + (2 * xRange) * (desiredPositions[i] / 10.0f);

            Instantiate(powerUpPrefabs[powerUpIndex], new Vector3(powerUpX, 1.0f, transform.position.z + 20), Quaternion.identity);
        }
    }

    
    void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    void SpawnEnemy()
    {
        float randomX = Random.Range(-xRange, xRange);
        float spawnY = 1.5f;
        GameObject enemy = Instantiate(enemyPrefab, new Vector3(randomX, spawnY, transform.position.z + 20), Quaternion.identity);

        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        if (enemyController != null)
        {
            enemyController.SetMovement(enemySpeed, despawnZ);
        }
    }


    void Update()
    {
        if (Time.time > nextSpawnTime)
        {
            enemiesSpawned++;
            if (enemiesSpawned % powerUpWaves == 0)
            {
                SpawnPowerUps();
            }
            else if (enemiesSpawned % powerUpWaves != 1)
            {
                SpawnEnemy();
            }

            nextSpawnTime = Time.time + spawnRate;
        }
    }


}