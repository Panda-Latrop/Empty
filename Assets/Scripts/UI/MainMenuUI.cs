using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : SimpleUI
{
    public string prologe, level;
    public void OnProloge()
    {
        GameInstance.Instance.LoadScene(prologe, 0, 0);
    }
    public void OnGame()
    {
        GameInstance.Instance.LoadScene(level, 0, 0);
    }
    public void OnExit()
    {
        Application.Quit();
    }
}
