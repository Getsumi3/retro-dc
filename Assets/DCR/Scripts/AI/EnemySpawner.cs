using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    public float spawnTime;
    public float waveDelay;
    public float spawnRadius = 5f;
    public GameObject[] enemies;
    public int enemiesPerWave;

    void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        yield return new WaitForSeconds(1f);
        while (true)
        {
            for (int i = 0; i < enemiesPerWave; i++)
            {
                Vector3 spawnPos = transform.position + Random.insideUnitSphere * spawnRadius;
                GameObject spawnedEnemy = Instantiate(enemies[Random.Range(0, enemies.Length)], spawnPos, Quaternion.identity);
                spawnedEnemy.transform.parent = this.transform;
                yield return new WaitForSeconds(spawnTime);
            }
            yield return new WaitForSeconds(waveDelay);
        }
    }
}
