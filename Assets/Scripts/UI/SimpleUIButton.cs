using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SimpleUIButton : MonoBehaviour, IPointerClickHandler
{
    // Start is called before the first frame update
    protected System.Action OnClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        CallOnClick();
    }
    public void CallOnClick()
    {
        OnClick?.Invoke();
    }
    public void BindOnClick(System.Action action)
    {
        OnClick += action;
    }
    public void UnbindOnClick(System.Action action)
    {
        OnClick -= action;
    }
    public void ClearOnClick()
    {
        OnClick = null;
    }
}
