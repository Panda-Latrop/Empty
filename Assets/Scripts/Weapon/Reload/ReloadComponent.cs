using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ReloadState
{
    initiated,
    process,
    ended,
    restart,
}

public class ReloadComponent : MonoBehaviour, ISaveableComponent
{
    protected bool isReload;
    [SerializeField]
    protected AmmunitionType type;
    [SerializeField]
    protected int maxClip;
    protected int currentClip = -1;
    protected AmmunitionComponent.AmmunitionClass ammunition;
    [SerializeField]
    protected float reloadTime  = 1.0f;
    protected ReloadState reloadState;
    protected float nextReload;
    [SerializeField]
    protected AudioSource source;
    [SerializeField]
    protected AudioClip reloadSound;


    public int Clip { get => currentClip; set => currentClip = value; }
    public int MaxClip => maxClip;
    public bool IsReload => isReload;
    public bool IsFull => currentClip >= maxClip;
    public bool IsEmpty => currentClip <= 0;
    public AmmunitionType Type => type;
    public ReloadState ReloadState => reloadState;

    protected virtual void Start()
    {
        if (currentClip < 0)
            currentClip = maxClip;
    }
    protected virtual void PlaySound()
    {
        source.Stop();
        source.clip = reloadSound;
        source.time = 0.0f;
        source.Play();
    }
    protected virtual void StopSound()
    {
        source.Stop();
    }
    public virtual void Increase()
    {
        currentClip++;
    }
    public virtual void Increase(int ammo)
    {
        currentClip = Mathf.Clamp(currentClip + ammo, 0, maxClip);
    }
    public virtual void Decrease()
    {
        currentClip--;
    }
    public virtual void Decrease(int ammo)
    {
        currentClip = Mathf.Clamp(currentClip - ammo ,0,maxClip);
    }
    public void SetAmmunition(AmmunitionComponent ammunition)
    {
        this.ammunition = ammunition.Get(type);
    }
    public virtual void Reload(bool reload)
    {
        //Debug.Log(!isReload + " " + reload + " " + (currentClip < maxClip) + " " + (!ammunition.IsEmpty));
        if (!isReload && reload && currentClip < maxClip && !ammunition.IsEmpty)
        {
            nextReload = Time.time + reloadTime;
            isReload = enabled = true;
            PlaySound();
            reloadState = ReloadState.initiated;
        }
    }
    public void Stop()
    {
        if (isReload)
        {
            isReload = false;
            enabled = false;
            StopSound();
        }
    }
    protected virtual void Update()
    {
        if (isReload)
        {
            if (Time.time >= nextReload)
            {
                int add = ammunition.Ammo - maxClip;
                if (add > 0)
                    add = 0;
                add = maxClip + add;
                currentClip = add;
                ammunition.Remove(add);
                reloadState = ReloadState.ended;
                isReload = false;
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

    public JSONObject Save(JSONObject jsonObject)
    {
        throw new System.NotImplementedException();
    }
    public JSONObject Load(JSONObject jsonObject)
    {
        throw new System.NotImplementedException();
    }
}
