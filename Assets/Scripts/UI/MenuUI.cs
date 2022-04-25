using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : SimpleUI
{
   // [SerializeField]
   // protected SimpleUIButton resume, toMain;

    public void Awake()
    {
        //resume.BindOnClick(OnResume);
      //  toMain.BindOnClick(ToMainMenu);
    }
    public void OnResume()
    {
        Time.timeScale = 1;       
        GameInstance.Instance.PlayerController.gameInput = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Hide();
        Debug.Log("Fuck");
    }
    public void ToMainMenu()
    {
        Time.timeScale = 1;
        GameInstance.Instance.LoadScene("mainmenu", 0, 0);
    }
}
