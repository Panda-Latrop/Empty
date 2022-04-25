using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponShooter : WeaponActor
{
    [SerializeField]
    protected float maxSpread, minSpread, spreadByShoot, spreadByTime;
    protected float spread;
    //protected RaycastHit hit;
    //protected int count;
    //[SerializeField]
    //protected WeaponTraceComponent trace;
    public virtual float Spread => spread;
    public override ShootState Shoot(Vector3 position, Vector3 direction)
    {
        
        if (CanShoot())
        {
            enabled = true;
            CreateProjectile(position, direction);
            PlaySound();
            PlayMuzzleFlash();
            IncreaseSpread();
            ammo.Decrease();
            shootState = ShootState.initiated;
            return shootState;
        }
        shootState = ShootState.unready;
        return shootState;
    }


    protected override HurtResult CreateProjectile(Vector3 position, Vector3 direction)
    {
        float angle = Mathf.Tan(Mathf.Deg2Rad * Spread * 0.5f);
        direction = Random.insideUnitSphere * angle + direction;
        direction.Normalize();
        HurtResult result;
        result = projectile.CreateProjectile(position, direction, distance, speed, new DamageStruct(owner.gameObject, owner.Health.Team, Damage, direction, Power));
        return result;
    }

    //protected override HurtResult CreateProjectile(Vector3 position, Vector3 direction)
    //{
    //    float angle = Mathf.Tan(Mathf.Deg2Rad * Spread * 0.5f);
    //    direction = Random.insideUnitSphere * angle + direction;
    //    direction.Normalize();

    //    Debug.DrawLine(position, position + direction * distance, Color.red, 1.0f);

    //    IHealth health;
    //    HurtResult result;
    //    if (Physics.Raycast(position, direction, out hit, distance, (1 << 6) | (1 << 7) | (1 << 8) | (1 << 10), QueryTriggerInteraction.Collide))
    //    {
    //        bool isPawnLayer = (1 << hit.collider.gameObject.layer) == (1 << 8);
    //        if (isPawnLayer)
    //        {
    //            health = hit.collider.GetComponent<IHealth>();
    //            result = health.Hurt(new DamageStruct(owner.gameObject, owner.Health.Team, Damage, direction, Power), hit);
    //            //Debug.Log(result.ToString());
    //        }
    //        else
    //        {
    //            result = HurtResult.miss;
    //        }
    //    }
    //    else
    //    {
    //        hit.point = position + direction * distance;
    //        result = HurtResult.none;
    //    }
    //    HitProcessing(hit, direction, Power, result);
    //    return hit;
    //}
    //protected override void HitProcessing(RaycastHit hit, Vector3 direction, float power, HurtResult result)
    //{
    //    base.HitProcessing(hit, direction, power, result);
    //    TraceActor trace = default;
    //    this.trace.Create(ref trace, shootPoint.position, hit.point);
    //}
    protected bool IncreaseSpread()
    {
        spread += spreadByShoot;
        if (spread > maxSpread)
        {
            spread = maxSpread;
            return true;
        }
        return false;
    }
    protected bool DecreaseSpread()
    {
        spread -= spreadByTime * Time.deltaTime;
        if (spread < minSpread)
        {
            spread = minSpread;
            return true;
        }
        return false;
    }
    protected virtual void Update()
    {
        if (!isFire && Time.time >= nextShoot)
        {
            if (DecreaseSpread())
                enabled = false;
        }
    }
}
