using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public AnimationCharacterComponent anim;
    public Transform target1, target2;
    public Vector3 dir1, dir2, aim;
    public Quaternion aimQ;

    public void Start()
    {
        anim.SetWeaponLayerWeight(1);
    }

    public void Update()
    {
        dir1 = (target1.position - transform.position).normalized;
        dir2 = (target2.position - transform.position).normalized;
        Vector3 dir3 = (dir2 - dir1).normalized;
        //aim.x = dir1.x * dir2.x;
        //aim.y = dir1.y * dir2.y;
        //aim.z = dir1.z * dir2.z;

        //aim.x = Mathf.Atan2(dir3.x, dir3.z) * Mathf.Rad2Deg / 90.0f;
        //aim.y = Mathf.Atan2(dir3.y, dir3.z) * Mathf.Rad2Deg / 90.0f;


        aimQ = Quaternion.FromToRotation(dir1, dir2);
        aim.x = -aimQ.y;
        aim.y = aimQ.x;
        anim.SetAim(aim);
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
