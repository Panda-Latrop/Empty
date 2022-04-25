using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test2 : MonoBehaviour
{
    public Transform target1,target2;
    public Vector3 dir1,dir2,aim;

    public void Update()
    {
        dir1 = (target1.position - transform.position).normalized;
        dir2 = (target2.position - transform.position).normalized;
    }

    private void OnDrawGizmos()
    {
        if (target1 == null || target2 == null)
            return;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + dir1);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + dir2);


    }
}
