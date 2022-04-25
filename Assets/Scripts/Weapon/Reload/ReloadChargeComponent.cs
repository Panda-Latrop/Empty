using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadChargeComponent : ReloadComponent
{
    [SerializeField]
    protected float chargePreFrame = -1;
    protected float charge;
    protected override void Start()
    {
        base.Start();
        if(chargePreFrame < 0.0f)
        chargePreFrame = maxClip * (1.0f / (60.0f* reloadTime));
    }
    protected override void PlaySound()
    {
        if (!isReload)
        {
            source.loop = true;
            base.PlaySound();
        }
    }
    public override void Reload(bool reload)
    {
        if(reload && currentClip < maxClip && !ammunition.IsEmpty)
        {
            PlaySound();
            isReload = true;
            charge += chargePreFrame;
            if (charge >= 1.0f)
            {
                int round = Mathf.RoundToInt(charge);
                charge -= round;
                int add = ammunition.Ammo - round;
                if (add > 0)
                    add = 0;
                add = round + add;
                currentClip += add;
                ammunition.Remove(add);
            }         
            reloadState = ReloadState.process;        
            if(currentClip >= maxClip || ammunition.IsEmpty)
            {
                isReload = false;
                reloadState = ReloadState.ended;
                StopSound();
            }
        }
        else
        {
            if (isReload)
            {
                isReload = false;
                reloadState = ReloadState.ended;
                StopSound();
            }
        }               
    }
    protected override void Update()
    {
        enabled = false;
    }
}
