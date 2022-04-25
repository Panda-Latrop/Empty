using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadPartComponent : ReloadComponent
{
    [SerializeField]
    protected int ammoInReload;

    protected override void Update()
    {
        if (isReload)
        {
            if (Time.time >= nextReload)
            {
                int add = ammunition.Ammo - ammoInReload;
                if (add > 0)
                    add = 0;
                add = ammoInReload + add;
                currentClip += add;
                ammunition.Remove(add);
                if(currentClip == maxClip || ammunition.IsEmpty)
                {

                    reloadState = ReloadState.ended;
                    isReload = false;
                }
                else
                {
                    reloadState = ReloadState.restart;
                    nextReload = Time.time + reloadTime;
                    PlaySound();
                }
            }
            else
            {
                reloadState = ReloadState.process;
            }
        }
        else
        {
            enabled = false;
            reloadState = ReloadState.ended;
        }
    }
}
