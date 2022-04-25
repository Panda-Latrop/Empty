using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCreateHitscanComponent : WeaponCreateBaseComponent
{
    protected RaycastHit hit;
    [SerializeField]
    protected bool useImpcat = false;
    [SerializeField]
    protected DynamicImpactActor impact;
    public override HurtResult CreateProjectile(Vector3 position, Vector3 direction, float distance, float speed, DamageStruct ds)
    {

        Debug.DrawLine(position, position + direction * distance, Color.red, 1.0f);
        IHealth health;
        HurtResult result;
        if (Physics.Raycast(position, direction, out hit, distance, (1 << 6) | (1 << 7) | (1 << 8) | (1 << 10), QueryTriggerInteraction.Collide))
        {
            bool isPawnLayer = (1 << hit.collider.gameObject.layer) == (1 << 8);
            if (isPawnLayer)
            {
                health = hit.collider.GetComponent<IHealth>();
                result = health.Hurt(ds, hit);
            }
            else
            {
                result = HurtResult.miss;
            }
        }
        else
        {
            hit.point = position + direction * distance;
            result = HurtResult.none;
        }
        HitProcessing(hit,position, direction, ds.power, result);
        return result;
    }
    protected virtual void HitProcessing(RaycastHit hit,Vector3 from, Vector3 direction, float power, HurtResult result)
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

        }
    }
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(hit.point, Vector3.one * 0.1f);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(hit.point, hit.point + hit.normal);
    }
}
