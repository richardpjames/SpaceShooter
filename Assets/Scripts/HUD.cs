using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] GameObject restartButton;
    [SerializeField] Slider healthBar;
    // Start is called before the first frame update
    void Awake()
    {
        EventManager.OnPlayerDead += ShowRestartButton;
        EventManager.OnHealthUpdated += UpdateHealthBar;
    }

    void OnDestroy()
    {
        EventManager.OnPlayerDead -= ShowRestartButton;
        EventManager.OnHealthUpdated -= UpdateHealthBar;
    }

    void Start()
    {
        HideRestartButton();
    }

    private void ShowRestartButton()
    {
        restartButton.SetActive(true);

    }

    private void HideRestartButton()
    {
        restartButton.SetActive(false);
    }

    public void RestartClicked()
    {
        EventManager.OnRestartRequested?.Invoke();
        HideRestartButton();
    }

    private void UpdateHealthBar()
    {
        healthBar.value = ((float) GameManager.Instance.GetCurrentHealth() / (float) GameManager.Instance.GetMaxHealth());
    }
}
