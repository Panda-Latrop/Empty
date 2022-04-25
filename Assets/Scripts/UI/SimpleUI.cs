using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleUI : MonoBehaviour
{
    [SerializeField]
    protected Canvas canvas;

    public void Show()
    {
        canvas.enabled = true;
    }
    public void Hide()
    {
        canvas.enabled = false;
    }

}
