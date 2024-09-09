using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }
    // These are the game events
    public static Action OnEnemyKilled;
    public static Action OnPlayerHit;
    public static Action OnRestartRequested;
    public static Action OnPlayerDead;
    public static Action OnHealthUpdated;

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
