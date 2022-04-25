using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShootState
{
    initiated,
    process,
    ended,
    unready,
    none,
}

public abstract class WeaponActor : Actor, IPoolObject
{
    [SerializeField]
    protected ItemWeapon item;
    protected Pawn owner;
    protected bool isFire;
    [SerializeField]
    protected bool isAutomatic;
    [SerializeField]
    protected Transform shootPoint;
    [SerializeField]
    protected float damage = 1.0f, damageMultiply = 1.0f;
    [SerializeField]
    protected float power = 100.0f, distance = 500.0f,speed = 5.0f;
    [SerializeField]
    protected float fireRate = 1.0f;
    [SerializeField]
    protected float changedTime = 1.0f;
    [SerializeField]
    protected AnimationCurveScriptableObject recoilCurve;
    [SerializeField]
    protected RuntimeAnimatorController fpAnimator, tpAnimator;
    [SerializeField]
    protected bool hasRenderer = true;
    [SerializeField]
    protected new Renderer renderer;
    [SerializeField]
    protected bool useMuzzleFlash = true;
    [SerializeField]
    protected ParticleSystem muzzleFlash;
    [SerializeField]
    protected AudioSource source;
    [SerializeField]
    protected AudioClip shootSound;
    [SerializeField]
    protected ReloadComponent ammo;
    [SerializeField]
    protected WeaponCreateBaseComponent projectile;


    protected float timeToShoot, nextShoot;
    protected ShootState shootState = ShootState.ended;

    public string PoolTag => GetType().Name + specifier;
    public bool IsFire => isFire;
    public virtual float Damage => damage * damageMultiply;
    public virtual float Power => power * damageMultiply;
    public float ChangedTime => changedTime;
    public ShootState ShootState => shootState;
    public float DamageMultiply { get => damageMultiply; set => damageMultiply = value; }
    public Transform ShootPoint => shootPoint;
    public ReloadComponent Ammo => ammo;
    public AnimationCurveScriptableObject RecoilCurve => recoilCurve;
    public Renderer Renderer => renderer;

    public void DisableShadow()
    {
        if (hasRenderer)
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }
    public void EnableShadow()
    {
        if (hasRenderer)
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    }
    protected virtual void Awake()
    {
        timeToShoot = 1.0f / fireRate;
    }

    public virtual void OnPush()
    {
        damageMultiply = 1.0f;
        gameObject.SetActive(false);
        ammo.Stop();
        StopSound();
        StopMuzzleFlash();
        return;
    }

    public virtual void OnPop()
    {
        return;
    }
    public void SetOwner(Pawn owner)
    {
        this.owner = owner;
    }
    public void SetAmmunition(AmmunitionComponent ammunition)
    {
        ammo.SetAmmunition(ammunition);
    }
    public virtual void SetFire(bool fire)
    {
        isFire = fire;
    }
    public virtual void OnChange()
    {
        ammo.Stop();
        StopMuzzleFlash();
        ///StopSound();
    }
    protected virtual void PlaySound()
    {
        source.Stop();
        source.clip = shootSound;
        source.time = 0.0f;
        source.Play();
    }
    protected virtual void StopSound()
    {
        source.loop = false;
        source.Stop();

    }
    protected virtual void PlayMuzzleFlash()
    {
        if (useMuzzleFlash)
        {
            muzzleFlash.Stop(true);
            muzzleFlash.time = 0.0f;
            muzzleFlash.Play(true);
        }
    }
    protected virtual void StopMuzzleFlash()
    {
        if (useMuzzleFlash)
        {
            muzzleFlash.Stop(true);
        }
    }
    protected virtual bool CanShoot()
    {
        if (isFire && !ammo.IsEmpty && Time.time >= nextShoot)
        {
            if (!isAutomatic)
                isFire = false;
            nextShoot = Time.time + timeToShoot;
            ammo.Stop();
            return true;
        }
        return false;
    }

    public virtual void Stop() { ammo.Stop(); }

    public virtual void Reload(bool reload = true)
    {
        StopMuzzleFlash();
        ammo.Reload(reload);
    }
    public abstract ShootState Shoot(Vector3 position, Vector3 direction);

    protected abstract HurtResult CreateProjectile(Vector3 position, Vector3 direction);

    //protected virtual void HitProcessing(RaycastHit hit, Vector3 direction, float power, HurtResult result)
    //{
    //    switch (result)
    //    {
    //        case HurtResult.none:
    //        case HurtResult.friend:
    //        case HurtResult.kill:
    //        case HurtResult.enemy:
    //            break;
    //        case HurtResult.miss:
    //            int layer = 1 << hit.collider.gameObject.layer;
    //            if (layer == (1 << 7) || layer == (1 << 10))
    //            {
    //                hit.rigidbody.AddForceAtPosition(direction * power, hit.point, ForceMode.Impulse);
    //            }                              
    //            break;
    //        default:
    //            break;
    //    }

    //}
    public void Restore()
    {
        ammo.Clip = ammo.MaxClip;
    }
    public RuntimeAnimatorController GetFirstPersonAnimatorController() => fpAnimator;
    public RuntimeAnimatorController GetThirdPersonAnimatorController() => tpAnimator;
    public virtual void Drop(Vector3 position, Quaternion rotation, Vector3 direction, float power)
    {
        var item =  GameInstance.Instance.PoolManager.Pop(this.item) as ItemWeapon;
        item.SetPosition(position);
        item.SetRotation(rotation);
        item.Rigidbody.AddForce(direction * power, ForceMode.Force);
        item.Rigidbody.AddTorque(Random.insideUnitSphere * power,ForceMode.Force);
        item.SetClip(ammo.Clip);
        GameInstance.Instance.PoolManager.Push(this);
    }
    public override JSONObject Save( JSONObject jsonObject)
    {
        jsonObject.Add("name", new JSONString(gameObject.name));
        jsonObject.Add("prefab", new JSONString(path));
        jsonObject.Add("enabled", new JSONBool(enabled));
        jsonObject.Add("active", new JSONBool(gameObject.activeSelf));
        //SaveSystem.TimerSave(jsonObject, "next", nextShoot);
        //jsonObject.Add("shootState", new JSONNumber((int)shootState));
        //jsonObject.Add("damageMultiply", new JSONNumber(damageMultiply));
        return jsonObject;
    }
    public override JSONObject Load( JSONObject jsonObject)
    {
        gameObject.name = jsonObject["name"];
        enabled = jsonObject["enabled"].AsBool;
        gameObject.SetActive(jsonObject["active"].AsBool);
        //SaveSystem.TimerLoad(jsonObject, "next", ref nextShoot);
        //shootState = (ShootState)jsonObject["shootState"].AsInt;
        //damageMultiply = jsonObject["damageMultiply"].AsFloat;
        return jsonObject;
    }
}
