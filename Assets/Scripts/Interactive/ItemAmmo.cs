using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAmmo : ItemActor
{
    [SerializeField]
    protected AmmunitionType type;
    [SerializeField]
    protected int ammo;

    public override void Pickup(Character by)
    {
        if (by.WeaponHolder.AddAmmo(type, ammo))
            GameInstance.Instance.PoolManager.Push(this);
    }
}
