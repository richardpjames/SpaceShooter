using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject enemySpawnParticles;
    [SerializeField] private Transform[] spawnPoints;
    // Start is called before the first frame update
    void Awake()
    {
        EventManager.OnEnemyKilled += Spawn;
    }
    private void OnDestroy()
    {
        EventManager.OnEnemyKilled -= Spawn;
    }

    // Update is called once per frame
    void Spawn()
    {
        // Pick a random spawn point
        int random = Random.Range(0, spawnPoints.Length);
        // Spawn an enemy at that point
        Instantiate(enemySpawnParticles, spawnPoints[random].position, Quaternion.identity);
        Instantiate(enemyPrefab, spawnPoints[random].position, Quaternion.identity);
    }
}
