using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
   public void StartClicked()
    {
        EventManager.OnRestartRequested?.Invoke();
    }

    public void QuitClicked()
    {
        GameManager.Instance.SaveAndQuit();
    }
}
