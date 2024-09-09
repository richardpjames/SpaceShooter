using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    // Get references to each of the key UI components
    [SerializeField] GameObject buttons;
    [SerializeField] Slider healthBar;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI gameOverText;


    // Determine which behaviour to trigger for key game events
    private void Awake()
    {
        EventManager.OnPlayerDead += ShowButtons;
        EventManager.OnHealthUpdated += UpdateHealthBar;
        EventManager.OnScoreUpdated += UpdateScore;

    }

    // Ensure no subscriptions remain after player death
    private void OnDestroy()
    {
        EventManager.OnPlayerDead -= ShowButtons;
        EventManager.OnHealthUpdated -= UpdateHealthBar;
        EventManager.OnScoreUpdated -= UpdateScore;
    }

    private void Start()
    {
        // When the game starts hide the UI buttons (restart and quit) and update the score
        HideButtons();
        scoreText.text = GameManager.Instance.GetScore().ToString();
    }

    // Shows all of the buttons on the screen (invoked by events)
    private void ShowButtons()
    {
        // Show the restart and quit buttons
        buttons.SetActive(true);
        // Update the text that shows for game over
        int score = GameManager.Instance.GetScore();
        int bestScore = GameManager.Instance.GetBestScore();
        // Messages determined by score
        string message = "";
        if(score > bestScore)
        {
            message = $"Game Over!\n Your Score is {GameManager.Instance.GetScore()}\n That's a New Best!";

        }
        else if (score == bestScore)
        {
            message = $"Game Over!\n Your Score is {GameManager.Instance.GetScore()}\n That's Equal to your Best!";
        }
        else
        {
            message = $"Game Over!\n Your Score is {GameManager.Instance.GetScore()}\n Your Best was {GameManager.Instance.GetBestScore()}";
        }
        gameOverText.text = message;
    }
    // Hides all of the buttons on the screen (invoked by events)
    private void HideButtons()
    {
        // Hide the restart and quit buttons
        buttons.SetActive(false);
    }

    // This happens when the restart button is clicked
    public void RestartClicked()
    {
        // Trigger the game manager to restart the game
        EventManager.OnRestartRequested?.Invoke();
        // Hide the buttons again
        HideButtons();
    }

    // This happens when the quit button is clicked
    public void QuitClicked()
    {
        // Simply quit the application
        EventManager.OnQuitRequested?.Invoke();
    }

    // When health is updated this updates the health bar
    private void UpdateHealthBar()
    {
        // Casting to floats allows for decimals to be calculated, then sets slider value for the healthbar
        healthBar.value = ((float)GameManager.Instance.GetCurrentHealth() / (float)GameManager.Instance.GetMaxHealth());
    }

    // When the score is updated this updates the text
    private void UpdateScore()
    {
        // Get the latest score from the game manager
        scoreText.text = GameManager.Instance.GetScore().ToString();
    }
}
