using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCreateProjectileComponent : WeaponCreateBaseComponent
{
    [SerializeField]
    protected ProjectileActor projectile;
    public override HurtResult CreateProjectile(Vector3 position, Vector3 direction, float distance, float speed, DamageStruct ds)
    {
        IPoolProjectile ipp = GameInstance.Instance.PoolManager.Pop(projectile) as IPoolProjectile;
        ipp.SetPosition(position);
        ipp.SetDamage(ds, speed);
        ipp.SetDirection(direction);
        //Debug.Break();
        return HurtResult.projectile;
    }
}
