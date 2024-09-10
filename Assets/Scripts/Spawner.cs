using UnityEngine;

public class Spawner : MonoBehaviour
{
    // Get reference to the enemy, particles on spawn and the spawn points on the map
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject enemySpawnParticles;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int difficultyIncrement;
    private int currentDifficultyLevel;

    // Need to know when an enemy dies so that we can spawn a new one
    void Awake()
    {
        EventManager.OnEnemyKilled += Spawn;
    }
    // Ensure that no subscriptions are left around
    private void OnDestroy()
    {
        EventManager.OnEnemyKilled -= Spawn;
    }

    // Update is called once per frame
    void Spawn()
    {
        int numberOfSpawns = 1;
        if (GameManager.Instance.GetScore() % difficultyIncrement == 0)
        {
            numberOfSpawns++;
            currentDifficultyLevel++;
        }
        for (int i = 0; i < numberOfSpawns; i++)
        {
            // Pick a random spawn point
            int random = Random.Range(0, spawnPoints.Length);
            // Spawn an enemy at that point
            Instantiate(enemySpawnParticles, spawnPoints[random].position, Quaternion.identity);
            Instantiate(enemyPrefab, spawnPoints[random].position, Quaternion.identity);
        }
    }
}
