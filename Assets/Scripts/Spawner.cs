using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // Get reference to the enemy, particles on spawn and the spawn points on the map
    [SerializeField] private GameObject enemySpawnParticles;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private List<Wave> waves;
    private int currentWave = -1;
    private int enemiesKilledThisWave = 0;
    private int enemiesInThisWave = 0;

    // Need to know when an enemy dies so that we can spawn a new one
    void Awake()
    {
        EventManager.OnEnemyKilled += EnemyKilled;
        EventManager.OnNewGame += NewGame;
    }
    // Ensure that no subscriptions are left around
    private void OnDestroy()
    {
        EventManager.OnEnemyKilled -= EnemyKilled;
        EventManager.OnNewGame -= NewGame;
    }

    private void EnemyKilled()
    {
        // Record that another enemy is killed and spawn
        enemiesKilledThisWave++;
        Spawn();
    }

    // On a new game reset and spawn from the start
    private void NewGame()
    {
        // Reset all variables
        currentWave = -1;
        enemiesInThisWave = 0;
        enemiesKilledThisWave = 0;
        // Spawn the first wave
        Spawn();
    }    

    // Update is called once per frame
    private void Spawn()
    {
        // If all of the enemies are killed (or on first instance these are both zero)
        if (enemiesKilledThisWave == enemiesInThisWave && waves.Count >= 1)
        {
            // Increment the wave number
            currentWave++;
            // Cap the wave at the final one
            currentWave = Mathf.Min(currentWave, waves.Count - 1);
            // Reset enemy counters
            enemiesInThisWave = waves[currentWave].enemies.Count;
            enemiesKilledThisWave = 0;
            // Spawn each of the enemies in this wave at a random point
            foreach (GameObject enemy in waves[currentWave].enemies)
            {
                // Pick a random spawn point
                int random = Random.Range(0, spawnPoints.Length);
                // Spawn an enemy at that point
                Instantiate(enemySpawnParticles, spawnPoints[random].position, Quaternion.identity);
                Instantiate(enemy, spawnPoints[random].position, Quaternion.identity);
            }
        }
    }

    // For storing details of each wave of enemies
    [System.Serializable]
    public struct Wave
    {
        // The enemies in this wave
        public List<GameObject> enemies;
    }
}


