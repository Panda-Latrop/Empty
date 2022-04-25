using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchSmoothComponent : CrouchComponent
{
    //[SerializeField]
    //protected Transform head;
    //protected Vector3 currentPosition, defaultPosition;

    //protected void Start()
    //{
    //    currentPosition = defaultPosition = head.localPosition;
    //}



    protected override void SitDown()
    {
        if (movement.Grounded)
        {
            currentHeight -= riseSpeed * Time.fixedDeltaTime;
            //currentPosition.y -= riseSpeed * Time.fixedDeltaTime;
            if (currentHeight <= crouchHeight)
            {
                currentHeight = crouchHeight;
                //currentPosition.y = defaultPosition.y - (defaultHeight - currentHeight);
            }
        }
        else
        {
            currentHeight = crouchHeight;
            //currentPosition.y = defaultPosition.y - (defaultHeight - currentHeight);
            movement.Teleport(Vector3.up * (defaultHeight - crouchHeight));
        }

        capsule.center = Vector3.up * (defaultOffset - (defaultHeight - currentHeight) * 0.5f);
        capsule.height = currentHeight;
        //head.localPosition = currentPosition;
    }
    protected override void StandUp()
    {
        RaycastHit hit;
        Vector3 center = capsule.bounds.center;
        Vector3 extents = capsule.bounds.extents;
        center.y += defaultOffset - capsule.center.y;
        if (!Physics.SphereCast(capsule.bounds.center, extents.x, Vector3.up, out hit, defaultHeight * 0.5f - extents.x, (1 << 6), QueryTriggerInteraction.Ignore))
        {
            if (movement.Grounded)
            {

                currentHeight += riseSpeed * Time.fixedDeltaTime;
                //currentPosition.y += riseSpeed * Time.fixedDeltaTime;
                if (currentHeight >= defaultHeight)
                {
                    currentHeight = defaultHeight;
                    //currentPosition.y = defaultPosition.y;
                    enabled = false;
                }
            }
            else
            {
                currentHeight = defaultHeight;
                //currentPosition.y = defaultPosition.y;
                movement.Teleport(Vector3.down * (defaultHeight - crouchHeight));
                enabled = false;
            }
            capsule.center = capsule.center = Vector3.up * (defaultOffset - (defaultHeight - currentHeight) * 0.5f);
            capsule.height = currentHeight;
            //head.localPosition = currentPosition;
        }
    }
}