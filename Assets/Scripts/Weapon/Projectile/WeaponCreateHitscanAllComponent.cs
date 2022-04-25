using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCreateHitscanAllComponent : WeaponCreateHitscanComponent
{
    [SerializeField]
    protected Vector3 hurtboxHalfSize = Vector3.one * 0.5f;
    protected RaycastHit[] hits = new RaycastHit[8];
    protected int count;
    public override HurtResult CreateProjectile(Vector3 position, Vector3 direction,float distance,float speed, DamageStruct ds)
    {
        Debug.DrawLine(position, position + direction * distance, Color.red, 1.0f);
        int closest = 0;
        IHealth health = default;
        HurtResult result = HurtResult.none;
        count = Physics.BoxCastNonAlloc(position, hurtboxHalfSize, direction, hits, Quaternion.identity, distance, (1 << 6) | (1 << 7) | (1 << 8) | (1 << 10), QueryTriggerInteraction.Collide);
        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                if (hits[closest].distance >= hits[i].distance)
                {
                    bool isPawnLayer = (1 << hits[i].collider.gameObject.layer) == (1 << 8);
                    if (isPawnLayer)
                    {
                        IHealth ih = hits[i].collider.GetComponent<IHealth>();
                        if (ih.Team.Equals(ds.team))
                        {
                            if (closest == i)
                            {
                                if (++closest >= count)
                                    return HurtResult.none;
                            }
                            continue;
                        }
                        else
                        {
                            result = HurtResult.enemy;
                            health = ih;
                        }
                    }
                    else
                    {
                        result = HurtResult.miss;
                    }
                    closest = i;
                }
            }
            if (result.Equals(HurtResult.enemy))
            {
                result = health.Hurt(ds, hits[closest]);
            }
        }
        else
        {
            result = HurtResult.none;
            hits[closest].point = position + direction * distance;
        }
        //Debug.Log(result.ToString());
        HitProcessing(hits[closest],position, direction, ds.power, result);
        return result;
    }
    protected override void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            for (int i = 0; i < count; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawCube(hits[i].point, Vector3.one * 0.1f);
                Gizmos.color = Color.green;
                Gizmos.DrawLine(hits[i].point, hits[i].point + hits[i].normal);
            }
        }
    }
}