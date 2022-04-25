using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItem
{
    void Pickup(Character by);
}
public abstract class ItemActor : Actor, IItem, IPoolObject
{
    [SerializeField]
    protected new Rigidbody rigidbody;
    public Rigidbody Rigidbody => rigidbody;

    public string PoolTag => GetType().Name + specifier;

    public virtual void OnPop()
    {
        gameObject.SetActive(true);
        rigidbody.detectCollisions = true;
        rigidbody.velocity = Vector3.zero;
    }

    public virtual void OnPush()
    {
        rigidbody.velocity = Vector3.zero;
        rigidbody.detectCollisions = false;
        gameObject.SetActive(false);
    }

    public abstract void Pickup(Character by);
}
