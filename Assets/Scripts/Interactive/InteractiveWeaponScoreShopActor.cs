using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveWeaponScoreShopActor : InteractiveScoreShopBaseActor
{
    [SerializeField]
    protected WeaponActor weapon;
    [SerializeField]
    [Range(1,10)]
    protected int ammoMultiply = 1;

    protected override bool Buy(Character by)
    {
        WeaponHolderComponent holder = by.WeaponHolder;

        if (holder.HasSameWeapon(weapon))
        {
            if (!by.WeaponHolder.AmmoIsFull(weapon.Ammo.Type))
            {
                ReloadComponent reload = weapon.Ammo;
                holder.AddAmmo(reload.Type, reload.MaxClip * ammoMultiply);
                return true;    
            }else return false;
        }
        else
        {
            holder.Add(this.weapon,ref weapon, true);
            return true;
        }
    }
}
