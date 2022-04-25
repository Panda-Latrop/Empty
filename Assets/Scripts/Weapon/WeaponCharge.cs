using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCharge : WeaponShooter
{
    protected bool isCharge;
    [SerializeField]
    protected float timeToCharge;
    protected float chargePreFrame = -1;
    protected float charge, chargePercent;
    [SerializeField]
    protected AudioClip chargeSound;
    public override float Damage => damage * damageMultiply * (0.1f + chargePercent * 0.9f);
    public override float Power => power * chargePercent;
    public override float Spread => spread * chargePercent;

    protected void Start()
    {
        if (chargePreFrame < 0)
            chargePreFrame = ammo.MaxClip * (1.0f / (60.0f * timeToCharge));
        enabled = false;
    }
    protected virtual void PlayCharge()
    {
        if (!isCharge)
        {
            source.Stop();
            source.loop = true;
            source.time = 0.0f;
            source.clip = chargeSound;   
            source.Play();
        }
    }
    protected override void PlaySound()
    {
        source.loop = false;
        base.PlaySound();
    }
    protected override bool CanShoot()
    {
        if (isFire && !ammo.IsEmpty && Time.time >= nextShoot)
        {
            ammo.Stop();
            return true;
        }
        return false;
    }
    public override void OnChange()
    {
        base.OnChange();
        charge = 0;
        isCharge = false;
    }
    public override ShootState Shoot(Vector3 position, Vector3 direction)
    {
        if (CanShoot())
        {
            
            PlayCharge();
            isCharge = true;
            charge += chargePreFrame;
            chargePercent = charge / ammo.MaxClip;
            spread = maxSpread * (1.0f - (chargePercent));
            shootState = ShootState.process;
        }
        if (charge > 0 && (!isFire || charge >= ammo.Clip))
        {
            CreateProjectile(position, direction);
            ammo.Decrease(Mathf.RoundToInt(charge));
            PlaySound();
            PlayMuzzleFlash();
            charge = 0;
            chargePercent = 1;
            spread = maxSpread;
            shootState = ShootState.initiated;
            nextShoot = Time.time + timeToShoot;
            isCharge = false;
        }
        else
        {
            if(!isCharge)
            shootState = ShootState.unready;
        }
        return shootState;
    }

}
