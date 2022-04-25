using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMelee : WeaponActor
{
    protected bool inSwing;
    [SerializeField]
    protected float swingTime = 0.5f;
    //[SerializeField]
    //protected Vector3 hurtboxHalfSize = Vector3.one * 0.5f;
    protected float nextSwing;
    //protected RaycastHit[] hits = new RaycastHit[8];
    //protected int count;
    public override void OnPush()
    {
        base.OnPush();
        nextSwing = 0.0f;
        inSwing = false;
    }
    public override void OnChange()
    {
        base.OnChange();
        nextSwing = 0.0f;
        inSwing = false;
    }
    public override void Stop()
    {
        base.Stop();
        nextSwing = 0.0f;
        inSwing = false;
        enabled = false;
    }
    protected override void StopMuzzleFlash()
    {
    }
    protected override bool CanShoot()
    {
        if (isFire && !inSwing && !ammo.IsEmpty && Time.time >= nextShoot)
        {
            if (!isAutomatic)
                isFire = false;
            inSwing = true;
            nextSwing = Time.time + swingTime;
            nextShoot = Time.time + timeToShoot;
            ammo.Stop();
            return true;
        }
        return false;
    }

    public override ShootState Shoot(Vector3 position, Vector3 direction)
    {
        if (CanShoot())
        {
            enabled = true;
            PlaySound();
            shootState = ShootState.initiated;
        }
        else
        {
            if (inSwing)
                shootState = ShootState.process;
            else
                shootState = ShootState.unready;
        }
        return shootState;
    }

    protected override HurtResult CreateProjectile(Vector3 position, Vector3 direction)
    {
        HurtResult result;
        result = projectile.CreateProjectile(position, direction, distance, speed, new DamageStruct(owner.gameObject, owner.Health.Team, Damage, direction, Power));
        return result;
    }
    //protected override HurtResult CreateProjectile(Vector3 position, Vector3 direction)
    //{
    //    Debug.DrawLine(position, position + direction * distance, Color.red, 1.0f);
    //    int closest = 0;
    //    IHealth health = default;
    //    HurtResult result = HurtResult.none;
    //    count = Physics.BoxCastNonAlloc(position, hurtboxHalfSize, direction, hits, Quaternion.identity, distance, (1 << 6) | (1 << 7) | (1 << 8) | (1 << 10), QueryTriggerInteraction.Collide);
    //    if (count > 0)
    //    {
    //        for (int i = 0; i < count; i++)
    //        {
    //            if (hits[closest].distance >= hits[i].distance)
    //            {
    //                bool isPawnLayer = (1 << hits[i].collider.gameObject.layer) == (1 << 8);
    //                if (isPawnLayer)
    //                {
    //                    IHealth ih = hits[i].collider.GetComponent<IHealth>();
    //                    if (ih.Team.Equals(owner.Health.Team))
    //                    {                          
    //                        if (closest == i)
    //                        {
    //                            if (++closest >= count)
    //                                return new RaycastHit();
    //                        }
    //                        continue;
    //                    }
    //                    else
    //                    {
    //                        result = HurtResult.enemy;
    //                        health = ih;
    //                    }
    //                }
    //                else
    //                {
    //                    result = HurtResult.miss;
    //                }
    //                closest = i;
    //            }
    //        }
    //        if (result.Equals(HurtResult.enemy))
    //        {
    //            result = health.Hurt(new DamageStruct(owner.gameObject, owner.Health.Team, Damage, direction, Power), hits[closest]);
    //        }
    //    }
    //    else
    //    {
    //        result = HurtResult.none;
    //        hits[closest].point = position + direction * distance;
    //    }
    //    HitProcessing(hits[closest], direction, Power, result);
    //    return hits[closest];
    //}
    public override void Reload(bool reload = true){}
    protected void Update()
    {
        if (inSwing)
        {
            if (Time.time >= nextSwing)
            {
                CreateProjectile(owner.Look, owner.LookDirection);
                inSwing = false;
                enabled = false;
            }
        }
        else
        {
            enabled = false;
        }
    }
}
