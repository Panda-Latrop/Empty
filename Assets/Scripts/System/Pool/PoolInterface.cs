using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolObject
{
    string PoolTag { get; }
    string Path { get; }
    void OnPush();
    void OnPop();
    GameObject GameObject { get; }
    Transform Transform { get; }
    void SetPosition(Vector3 position);
    void SetRotation(Quaternion rotation);
    void SetScale(Vector3 scale);
    void SetParent(Transform parent);
}
public interface IPoolProjectile : IPoolObject
{
    void SetDamage(DamageStruct ds, float speed);
    void SetDirection(Vector3 direction);
}