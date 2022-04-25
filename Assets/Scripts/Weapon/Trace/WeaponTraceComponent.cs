using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTraceComponent : MonoBehaviour
{
    [SerializeField]
    protected TraceActor trace;
    [SerializeField]
    protected float minDistanceToCreate = 25.0f;

    public virtual bool Create(ref TraceActor trace ,Vector3 start, Vector3 end)
    {
        if ((start - end).sqrMagnitude > minDistanceToCreate* minDistanceToCreate)
        {
           // trace = GameInstance.Instance.PoolManager.Pop(this.trace) as TraceActor;
           // trace.SetLine(start, end);
            return true;
        }
        return false;
    }
    public virtual bool Create(ref TraceActor trace, Vector3 start, Vector3 end,Transform parent)
    {
       if(Create(ref trace,start,end))
        {
            trace.SetParent(parent);
            return true;
        }
        else
        {
            return false;
        }
    }
}
