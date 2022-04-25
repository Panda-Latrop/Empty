using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCreateHitscanTraceComponent : WeaponCreateHitscanComponent
{
    [SerializeField]
    protected TraceActor trace;
    protected override void HitProcessing(RaycastHit hit,Vector3 from, Vector3 direction, float power, HurtResult result)
    {
        base.HitProcessing(hit,from, direction, power, result);
        if (!result.Equals(HurtResult.friend) && !result.Equals(HurtResult.none) && !result.Equals(HurtResult.projectile))
        {
            if (useImpcat)
            {
                TraceActor t = GameInstance.Instance.PoolManager.Pop(trace) as TraceActor;
                t.SetLine(from, hit.point,direction , hit.distance * 0.30f);
            }

        }
    }
}
