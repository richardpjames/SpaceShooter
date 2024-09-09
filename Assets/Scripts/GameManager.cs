using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private int maxHealth = 50;
    private int currentHealth;
    // Start is called before the first frame update
    void Awake()
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

        EventManager.OnPlayerHit += PlayerHit;
        EventManager.OnRestartRequested += ResetLevel;
    }

    void OnDestroy()
    {
        EventManager.OnPlayerHit -= PlayerHit;
        EventManager.OnRestartRequested -= ResetLevel;
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void PlayerHit()
    {
        currentHealth--;
        if(currentHealth <= 0)
        {
            EventManager.OnPlayerDead?.Invoke();
        }
    }

    private void ResetLevel()
    {
        // Reset the players health back to the max
        currentHealth = maxHealth;
        // Reload the level
        SceneManager.LoadScene("Level");
    }

}
