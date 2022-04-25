using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicActor : Actor, IPoolObject
{
    protected float timeToPush = 2.0f;
    protected float nextPush;

    public string PoolTag => this.GetType().Name + specifier;

    public virtual void OnPop()
    {
        gameObject.SetActive(true);
        nextPush = Time.time + timeToPush;
        enabled = true;
    }

    public virtual void OnPush()
    {
        gameObject.SetActive(false);
    }

    protected void Start()
    {
        nextPush = Time.time + timeToPush;
    }
    protected void LateUpdate()
    {
        if(Time.time >= nextPush)
        {
            GameInstance.Instance.PoolManager.Push(this);
        }
    }
}
