using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TriggerActor : Actor
{
    [SerializeField]
    protected bool closeOnEnter = true;
    [SerializeField]
    protected new Collider collider;
    protected System.Action OnExecute;


    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Equals(GameInstance.Instance.PlayerController.ControlledPawn.name))
        {
            Execute(other);
            if (closeOnEnter)
            collider.enabled = false;
        }
    }
    protected virtual void Execute(Collider other)
    {
        CallOnExecute();
    }
    public void CallOnExecute()
    {
        OnExecute?.Invoke();
    }
    public void BindOnExecute(System.Action action)
    {
        OnExecute += action;
    }
    public void UnbindOnExecute(System.Action action)
    {
        OnExecute -= action;
    }
    public void ClearOnResurrect()
    {
        OnExecute = null;
    }
    protected void OnDestroy()
    {
        OnExecute = null;
    }
}
