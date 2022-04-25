using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ControllerBase : Actor, IPoolObject
{
    protected bool hasPawn;
    protected Pawn controlledPawn;
    public bool HasPawn { get => hasPawn; }
    public Pawn ControlledPawn { get => controlledPawn; }
    public string PoolTag => this.GetType().Name + specifier;

    public virtual void Possess(Pawn pawn)
    {
        gameObject.SetActive(true);
        enabled = true;
        controlledPawn = pawn;
        pawn.OnPossess(this);
        hasPawn = true;
        controlledPawn.Health.BindOnHurt(OnHurt);
        controlledPawn.Health.BindOnDeath(OnDeath);
        saveTag = pawn.SaveTag;
    }
    public virtual void Unpossess()
    {
        gameObject.SetActive(false);
        enabled = false;
        if (hasPawn)
        {
            controlledPawn.Health.UnbindOnHurt(OnHurt);
            controlledPawn.Health.UnbindOnDeath(OnDeath);
            controlledPawn.OnUnpossess();
            controlledPawn = null;
            hasPawn = false;
        }
        GameInstance.Instance.PoolManager.Push(this);
    }

    public virtual void OnPush()
    {
        gameObject.SetActive(false);
    }

    public virtual void OnPop()
    {
        gameObject.SetActive(true);
    }


    protected virtual void OnHurt(DamageStruct ds, RaycastHit raycastHit) { }
    protected virtual void OnDeath(DamageStruct ds, RaycastHit raycastHit) { }
    public override JSONObject Save( JSONObject jsonObject)
    {
        base.Save( jsonObject);
        jsonObject.Add("hasPawn", new JSONBool(hasPawn));
        if (hasPawn)
            SaveSystem.ComponentReferenceSave(jsonObject, string.Empty, controlledPawn);
        return jsonObject;
    }
    public override JSONObject Load( JSONObject jsonObject)
    {
        base.Load( jsonObject);
        if(hasPawn)
            return jsonObject;
        hasPawn = jsonObject["hasPawn"].AsBool;
        if(hasPawn)
        {
            Pawn pawn = default;
            if(hasPawn = SaveSystem.ComponentReferenceLoad(jsonObject, string.Empty,ref pawn))
            {
                Possess(pawn);
            }
        }
        return jsonObject;
    }
    protected virtual void OnDestroy()
    {
        if (!GameInstance.ApplicationIsQuit && !GameInstance.ChangeScene && hasPawn)
        {
                Unpossess();
        }
    }
}
