using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }
    // When an enemy is killed
    public static Action OnEnemyKilled;
    // When a projectile hits a player
    public static Action OnPlayerHit;
    // When the user requests the game is restarted
    public static Action OnRestartRequested;
    // When a new game is started
    public static Action OnNewGame;
    // When the player is dead
    public static Action OnPlayerDead;
    // When the players health is updated
    public static Action OnHealthUpdated;
    // When the score is updated
    public static Action OnScoreUpdated;
    // To save and quit the game
    public static Action OnQuitRequested;


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
    }
}
