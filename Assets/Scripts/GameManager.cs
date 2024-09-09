using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Holds an instance of the game manager for us elsewhere
    public static GameManager Instance { get; private set; }
    // Maximum health for the player
    [SerializeField] private int maxHealth = 50;
    // Current health and score
    private int currentHealth;
    private int score;

    private void Awake()
    {
        // Ensure that this is the only instance
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        // Subscribe to required events
        EventManager.OnPlayerHit += PlayerHit;
        EventManager.OnRestartRequested += ResetLevel;
        EventManager.OnEnemyKilled += UpdateScore;
    }

    // When destroying the object do not leave behind any subscriptions
    private void OnDestroy()
    {
        EventManager.OnPlayerHit -= PlayerHit;
        EventManager.OnRestartRequested -= ResetLevel;
        EventManager.OnEnemyKilled -= UpdateScore;
    }

    // Called at the very beginning of the game
    private void Start()
    {
        // Set the health to the max and the score to zero
        currentHealth = maxHealth;
        score = 0;
    }

    private void PlayerHit()
    {
        // If the player is hit then decrease their health by 1 (don't allow less than zero health)
        currentHealth--;
        currentHealth = Math.Max(currentHealth, 0);
        // Let other components know that the health has been updated
        EventManager.OnHealthUpdated?.Invoke();
        // If the health is less than zero then tell other components the player is dead
        if (currentHealth <= 0)
        {
            EventManager.OnPlayerDead?.Invoke();
        }
    }

    private void UpdateScore()
    {
        // Update the score whenever an enemy is killed
        score++;
        // Let other components know
        EventManager.OnScoreUpdated?.Invoke();
    }

    // Get the current score - used by UI components etc.
    public int GetScore()
    { 
        return score; 
    }

    // At the end of the game reset and load the level again
    private void ResetLevel()
    {
        // Reset the players health back to the max
        currentHealth = maxHealth;
        // Reset the score to zero
        score = 0;
        // Reload the level
        SceneManager.LoadScene("Level");
    }

    // Used by the healthbar to determine what percentage of health remains
    public int GetMaxHealth()
    {
        return maxHealth;
    }

    // Used by the healthbar to determine remaining health
    public int GetCurrentHealth()
    {
        return currentHealth;
    }

}
