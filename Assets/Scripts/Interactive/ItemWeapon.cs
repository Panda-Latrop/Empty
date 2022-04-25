using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWeapon : ItemActor, IInteractive
{
    [SerializeField]
    protected WeaponActor weapon;
    protected int clip = -1;
    [SerializeField]
    protected bool useSound = false;
    [SerializeField]
    protected DynamicSoundActor dynamicSound;
    protected void Start()
    {
        if (clip == -1)
            clip = weapon.Ammo.Clip;
    }

    public void SetClip(int clip)
    {
        this.clip = clip;
    }
    public override void OnPop()
    {
        base.OnPop();
        clip = weapon.Ammo.MaxClip;
    }

    public override void OnPush()
    {
        base.OnPush();
        clip = weapon.Ammo.MaxClip;
    }
    public virtual void Intercat(Character by)
    {
        Pickup(by);
    }
    public override void Pickup(Character by)
    {
        if (!by.WeaponHolder.HasSameWeapon(weapon))
        {
            WeaponActor weapon = default;
            if (by.WeaponHolder.Add(this.weapon, ref weapon, clip))
            {
                if (useSound)
                {
                    var ipo = GameInstance.Instance.PoolManager.Pop(dynamicSound);
                    ipo.SetPosition(transform.position);
                }
                GameInstance.Instance.PoolManager.Push(this);
            }

        }
    }


}
