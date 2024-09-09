using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] GameObject restartButton;
    // Start is called before the first frame update
    void Awake()
    {
        EventManager.OnPlayerDead += ShowRestartButton;
    }

    void OnDestroy()
    {
        EventManager.OnPlayerDead -= ShowRestartButton;
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
}
