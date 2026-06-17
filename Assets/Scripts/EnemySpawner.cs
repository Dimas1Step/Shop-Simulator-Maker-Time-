using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;

    public float width = 10f;
    public float length = 10f;
    public int enemyCount = 5;

    private void Start()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            SpawnEnemy();
        }
    }

    public void SpawnEnemy()
    {
        float randomX = Random.Range(-width / 2, width / 2);
        float randomZ = Random.Range(-length / 2, length / 2);

        Vector3 spawnPos = transform.position + new Vector3(randomX, 0, randomZ);

        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }
}