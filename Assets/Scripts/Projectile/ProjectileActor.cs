using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileActor : Actor, IPoolProjectile
{
    [SerializeField]
    protected new Rigidbody rigidbody;
    [SerializeField]
    protected float rayLength = 2.0f;
    [SerializeField]
    protected ProjectileMovementComponent projectileMovement;
    protected RaycastHit hit;
    protected DamageStruct ds;
    [SerializeField]
    protected float timeToPush = 10.0f;
    protected float nextPush;
    [SerializeField]
    protected bool useImpcat = false;
    [SerializeField]
    protected DynamicImpactActor impact;

    protected void Awake()
    {
        Setup();
    }
    protected virtual void Setup()
    {
        projectileMovement.SetRigidbody(rigidbody);
    }

    public string PoolTag => this.GetType().Name + specifier;
    public virtual void SetDamage(DamageStruct ds, float speed)
    {
        this.ds = ds;
        projectileMovement.Speed = speed;
    }
    public virtual void SetDirection(Vector3 direction)
    {
        projectileMovement.Move(direction);
    }
    public virtual void OnPop()
    {
        gameObject.SetActive(true);
        enabled = true;
        nextPush = timeToPush + Time.time;
    }
    public virtual void OnPush()
    {
        gameObject.SetActive(false);
        enabled = false;
        projectileMovement.Stop();
    }
    protected virtual void HitProcessing(RaycastHit hit, Vector3 direction, float power, HurtResult result)
    {

        if (!result.Equals(HurtResult.friend) && !result.Equals(HurtResult.none) && !result.Equals(HurtResult.projectile))
        {
            if (!result.Equals(HurtResult.kill))
            {
                if (result.Equals(HurtResult.miss))
                {
                    int layer = 1 << hit.collider.gameObject.layer;
                    if (layer == (1 << 7) || layer == (1 << 10))
                    {
                        hit.rigidbody.AddForceAtPosition(direction * power, hit.point, ForceMode.Impulse);
                    }
                }
            }
            if (useImpcat)
            {
                var ipo = GameInstance.Instance.PoolManager.Pop(impact);
                ipo.SetPosition(hit.point);
            }
            GameInstance.Instance.PoolManager.Push(this);

        }
    }
    protected void FixedUpdate()
    {
        CheckCollision();
        if (Time.time >= nextPush)
        {
            GameInstance.Instance.PoolManager.Push(this);
        }
    }
    protected virtual void CheckCollision()
    {
        IHealth health;
        HurtResult result;
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;
        if (Physics.Raycast(origin - direction * rayLength*0.5f,  direction , out hit, rayLength * 2, (1 << 6) | (1 << 7) | (1 << 8) | (1 << 10), QueryTriggerInteraction.Collide))
        {
            bool isPawnLayer = (1 << hit.collider.gameObject.layer) == (1 << 8);
            if (isPawnLayer)
            {
                health = hit.collider.GetComponent<IHealth>();
                ds.direction = direction;
                result = health.Hurt(ds, hit);
            }
            else
            {
                result = HurtResult.miss;
            }
            HitProcessing(hit, direction, ds.power, result);
            return;
        }

    }
    public override JSONObject Save(JSONObject jsonObject)
    {
        base.Save(jsonObject);

        return jsonObject;
    }
    public override JSONObject Load(JSONObject jsonObject)
    {
        base.Load(jsonObject);
        return jsonObject;
    }
    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position- transform.forward * rayLength, transform.position + transform.forward * rayLength);
    }
}
