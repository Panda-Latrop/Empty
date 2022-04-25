using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pawn : Actor, IPerceptionTarget, IPoolObject
{
    [SerializeField]
    protected ControllerBase autoController;
    protected bool hasAutoController,hasController;
    protected ControllerBase controller;
    [SerializeField]
    protected Transform lookTransform;
    [SerializeField]
    protected HealthComponent health;

    public Pawn Self => this;
    public bool HasController { get => hasController; }
    public ControllerBase Controller { get => controller; }
    public HealthComponent Health { get => health; }

    protected virtual void Setup()
    {
        health.BindOnHurt(OnHurt);
        health.BindOnDeath(OnDeath);
        health.BindOnResurrect(OnResurrect);
    }

    protected virtual void Awake()
    {
        Setup();
    }

    protected virtual void Start()
    {
        if (!hasController && (hasAutoController = autoController != null) && health.IsAlive)
        {
            (GameInstance.Instance.PoolManager.Pop(autoController) as ControllerBase).Possess(this);
        }
    }
    public virtual void OnPush()
    {
        gameObject.SetActive(false);
    }

    public virtual void OnPop()
    {
        gameObject.SetActive(true);
    }
    public virtual void OnPossess(ControllerBase controller)
    {
        hasController = true;
        this.controller = controller;
        return;
    }
    public virtual void OnUnpossess()
    {
        if (hasController)
        {
            hasController = false;
            controller = null;
        }
        return;
    }
    public virtual Vector3 Look => lookTransform.position;
    public virtual Vector3 LookDirection => lookTransform.forward;

    public string PoolTag => this.GetType().Name + specifier;

    public virtual void LookRotate(Vector3 byAngle)
    {
        lookTransform.rotation *= Quaternion.AngleAxis(byAngle.x, Vector3.right) * Quaternion.AngleAxis(byAngle.y, Vector3.up) * Quaternion.AngleAxis(byAngle.z, Vector3.forward);
    }
    public virtual void LookRotate(Vector3 direction, float displacement)
    {
        lookTransform.rotation *= Quaternion.FromToRotation(lookTransform.forward, direction);
    }
    protected virtual void OnHurt(DamageStruct ds, RaycastHit raycastHit)
    {
        return;
    }
    protected virtual void OnDeath(DamageStruct ds, RaycastHit raycastHit)
    {
        return;
    }
    protected virtual void OnResurrect()
    {
        health.Health = health.MaxHealth;
        if (!hasController && hasAutoController)
        {
            (GameInstance.Instance.PoolManager.Pop(autoController) as ControllerBase).Possess(this);
        }
        return;
    }
    public override JSONObject Save(JSONObject jsonObject)
    {
        base.Save(jsonObject);
        jsonObject.Add("hasController", new JSONBool(hasController));
        if (hasController)
            SaveSystem.ComponentReferenceSave(jsonObject, string.Empty, controller);
        jsonObject.Add("health", health.Save(new JSONObject()));
        return jsonObject;
    }
    public override JSONObject Load(JSONObject jsonObject)
    {
        base.Load(jsonObject);
        if (hasController = jsonObject["hasController"].AsBool)
        {
            hasController = SaveSystem.ComponentReferenceLoad(jsonObject, string.Empty, ref controller);
        }
        health.Load(jsonObject["health"].AsObject);
        return jsonObject;
    }
    protected virtual void OnDestroy()
    {

        health.UnbindOnHurt(OnHurt);
        health.UnbindOnDeath(OnDeath);
        health.UnbindOnResurrect(OnResurrect);

        if (!GameInstance.ApplicationIsQuit && !GameInstance.ChangeScene && hasController)
        {
            controller.Unpossess();
        }
    }

 
}
