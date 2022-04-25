using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIContainer : MonoBehaviour
{
    [SerializeField]
    protected List<SimpleUI> simpleUIs = new List<SimpleUI>();

    public bool GetScreen(string name, ref SimpleUI screen)
    {
        for (int i = 0; i < simpleUIs.Count; i++)
        {
            screen = simpleUIs[i];
            if (screen.name.Equals(name))
                return true;
        }
        return false;
    }

    public void ShowScreen(string name)
    {
        for (int i = 0; i < simpleUIs.Count; i++)
        {
            if (simpleUIs[i].name.Equals(name))
            {
                simpleUIs[i].Show();
            }
            else
            {
                simpleUIs[i].Hide();
            }
        }
    }
}
