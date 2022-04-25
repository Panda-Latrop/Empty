using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HurtResult
{
    none,
    miss,
    friend,
    enemy,
    kill,
    projectile
}
public enum Team
{
    world,
    team0,
    team1,
    team2,
}
public interface IHealth
{
    bool IsAlive { get; }
    HurtResult Hurt(DamageStruct ds, RaycastHit raycastHit);
    void Heal(float health);
    Team Team { get; set; }
}
[System.Serializable]
public struct DamageStruct
{
    public GameObject causer;
    public Team team;
    public float damage;
    public Vector3 direction;
    public float power;
    public string bone;
    //public Vector3 postition;
    //public Vector3 normal;
    public DamageStruct(GameObject causer, Team team, float damage, Vector3 direction, float power)
    {
        this.causer = causer;
        this.team = team;
        this.damage = damage;
        this.direction = direction;
        this.power = power;
        bone = string.Empty;
        //postition = Vector3.zero;
        //normal = Vector3.zero;
    }
}
public delegate void DamageDelegate(DamageStruct ds, RaycastHit raycastHit);
public class HealthComponent : MonoBehaviour , IHealth, ISaveableComponent
{
    [SerializeField]
    protected bool isAlive = true;
    [SerializeField]
    protected bool isImmortal;
    [SerializeField]
    protected float maxHealth = 100;
    protected float currentHealth = 0;
    [SerializeField]
    [Range(-1.0f, 1.0f)]
    protected float resistance = 0.0f;
    protected float currentResistance = -2.0f;
    [SerializeField]
    protected Team team;
    protected Team currentTeam;

    protected DamageDelegate OnHurt;
    protected DamageDelegate OnDeath;
    protected System.Action OnResurrect;

    public bool IsAlive { get => isAlive; }
    public bool IsImmortal { get => isImmortal; }
    public float Health { get => currentHealth; set {currentHealth = value;if (currentHealth > maxHealth) currentHealth = maxHealth;} }
    public float MaxHealth => maxHealth;
    public float Resistance { get => currentResistance; set { currentResistance = Mathf.Clamp(value ,- 1.0f,1.0f); } }
    public Team Team { get => currentTeam;  set => currentTeam = value; }
    public void Start()
    {
        if (currentHealth <= 0)
            currentHealth = maxHealth;
        if (currentTeam.Equals(Team.world))
            currentTeam = team;
        if (currentResistance == -2)
            currentResistance = resistance;
    }
    public HurtResult Hurt(DamageStruct ds, RaycastHit raycastHit)
    {
        if (isAlive)
        {
            if (ds.team != currentTeam || (ds.team == Team.world && currentTeam == Team.world))
            {
                if (!isImmortal)
                {
                    
                    currentHealth = currentHealth - ds.damage * (1 - currentResistance);
                    //Debug.Log(ds.damage);
                    if (currentHealth <= 0)
                    {
                        isAlive = false;
                        CallOnDeath(ds, raycastHit);
                        return HurtResult.kill;
                    }
                }
                CallOnHurt(ds, raycastHit);
                return HurtResult.enemy;
            }
            else
            {
                return HurtResult.friend;
            }
        }
        return HurtResult.none;
    }
    public void Heal(float health)
    {
        if (isAlive)
        {
            currentHealth += health;
            if (currentHealth > maxHealth)
                currentHealth = maxHealth;
        }
    }
  
    [ContextMenu("Kill")]
    public HurtResult Kill()
    {
        if (isAlive)
        {
            isAlive = false;
            currentHealth = 0;
            CallOnDeath(new DamageStruct(gameObject, Team.world, maxHealth, Vector3.zero, 0), new RaycastHit());
            return HurtResult.kill;
        }
        return HurtResult.miss;
    }
    public void Resurrect()
    {
        if (!isAlive)
        {
            isAlive = true;
            currentHealth = maxHealth;
            currentResistance = resistance;
            currentTeam = team;
            CallOnResurrect();
        }
    }

    public void CallOnHurt(DamageStruct ds, RaycastHit raycastHit)
    {
        OnHurt?.Invoke(ds, raycastHit);
    }
    public void BindOnHurt(DamageDelegate action)
    {
        OnHurt += action;
    }
    public void UnbindOnHurt(DamageDelegate action)
    {
        OnHurt -= action;
    }
    public void ClearOnHurt()
    {
        OnHurt = null;
    }

    public void CallOnDeath(DamageStruct ds, RaycastHit raycastHit)
    {
        OnDeath?.Invoke(ds, raycastHit);
    }
    public void BindOnDeath(DamageDelegate action)
    {
        OnDeath += action;
    }
    public void UnbindOnDeath(DamageDelegate action)
    {
        OnDeath -= action;
    }
    public void ClearOnDeath()
    {
        OnDeath = null;
    }
    public void CallOnResurrect()
    {
        OnResurrect?.Invoke();
    }
    public void BindOnResurrect(System.Action action)
    {
        OnResurrect += action;
    }
    public void UnbindOnResurrect(System.Action action)
    {
        OnResurrect -= action;
    }
    public void ClearOnResurrect()
    {
        OnResurrect = null;
    }
    protected void OnDestroy()
    {
        OnHurt = null;
        OnDeath = null;
        OnResurrect = null;
    }
    public virtual JSONObject Save( JSONObject jsonObject)
    {
        jsonObject.Add("isAlive", new JSONBool(isAlive));
        jsonObject.Add("currentHealth", new JSONNumber(currentHealth));
        jsonObject.Add("currentResistance", new JSONNumber(currentResistance));
        jsonObject.Add("team", new JSONNumber((int)currentTeam));
        return jsonObject;
    }
    public virtual JSONObject Load( JSONObject jsonObject)
    {
        isAlive = jsonObject["isAlive"].AsBool;
        currentHealth = jsonObject["currentHealth"].AsFloat;
        currentResistance = jsonObject["currentResistance"].AsFloat;
        currentTeam = (Team)jsonObject["team"].AsInt;
        return jsonObject;
    }
}
