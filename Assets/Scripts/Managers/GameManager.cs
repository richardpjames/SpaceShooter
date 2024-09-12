using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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
    private int bestScore;

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
        EventManager.OnRestartRequested += ResetLevel;
        EventManager.OnEnemyKilled += UpdateScore;
        EventManager.OnQuitRequested += SaveAndQuit;
    }

    // When destroying the object do not leave behind any subscriptions
    private void OnDestroy()
    {
        EventManager.OnRestartRequested -= ResetLevel;
        EventManager.OnEnemyKilled -= UpdateScore;
        EventManager.OnQuitRequested -= SaveAndQuit;
    }

    // Called at the very beginning of the game
    private void Start()
    {
        // Set the health to the max and the score to zero
        currentHealth = maxHealth;
        score = 0;
        // Load the game and set our previous best
        bestScore = 0;
        Save save = LoadGame();
        if (save != null)
        {
            bestScore = save.bestScore;
        }
        // Notify that a new game has started
        EventManager.OnNewGame?.Invoke();
    }

    public void EndGame()
    {
        // Signal out to all components that the game has ended
        EventManager.OnPlayerDead?.Invoke();
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
        // Check if we now have a new best
        if (score > bestScore)
        {
            // Update if we do
            bestScore = score;
        }
        // Reset the players health back to the max
        currentHealth = maxHealth;
        // Reset the score to zero
        score = 0;
        // Reload the level
        StartCoroutine(LoadLevel("Level"));
    }

    private static IEnumerator LoadLevel(string sceneName)
    {
        // Load th el
        AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        while (!asyncLoadLevel.isDone)
        {
            yield return null;
        }
        // Signal that a new game is started
        EventManager.OnNewGame?.Invoke();
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

    // Used by the UI to update the player
    public int GetBestScore()
    {
        return bestScore;
    }

    // Save the game and quit when requested
    public void SaveAndQuit()
    {
        // Create a save and write it to disk
        Save save = new Save();
        save.bestScore = bestScore;
        // Save to disk
        SaveGame(save);
        // Quit the application
        Application.Quit();
    }

    // Serialise and save the game
    private void SaveGame(Save save)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/SpaceShooter.save");
        binaryFormatter.Serialize(file, save);
        file.Close();
    }

    // Deserialise and load the game
    private Save LoadGame()
    {
        // Only load the file if it exists
        if (File.Exists(Application.persistentDataPath + "/SpaceShooter.save"))
        {
            // Read the data into a "save" object and return
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/SpaceShooter.save", FileMode.Open);
            Save save = (Save)binaryFormatter.Deserialize(file);
            file.Close();
            return save;
        }
        // If the file is not found then return null
        return null;
    }
}
