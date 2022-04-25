using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchPlayerComponent : CrouchSmoothComponent
{
    [SerializeField]
    protected new Transform camera;
    protected Vector3 defaultPosition;

    protected void Start()
    {
        defaultPosition = camera.localPosition;
    }



    protected override void SitDown()
    {
        if (movement.Grounded)
        {
            currentHeight -= riseSpeed * Time.fixedDeltaTime;
            if (currentHeight <= crouchHeight)
            {
                currentHeight = crouchHeight;
            }
        }
        else
        {
            currentHeight = crouchHeight;
            movement.Teleport(Vector3.up * (defaultHeight - crouchHeight));
        }

        camera.localPosition = Vector3.up * (defaultPosition.y - (defaultHeight - currentHeight));
        capsule.center = Vector3.up * (defaultOffset - (defaultHeight - currentHeight) * 0.5f);
        capsule.height = currentHeight;

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
                if (currentHeight >= defaultHeight)
                {
                    currentHeight = defaultHeight;
                    enabled = false;
                }
            }
            else
            {
                currentHeight = defaultHeight;
                movement.Teleport(Vector3.down * (defaultHeight - crouchHeight));
                enabled = false;
            }
            camera.localPosition = Vector3.up * (defaultPosition.y - (defaultHeight - currentHeight));
            capsule.center = capsule.center = Vector3.up * (defaultOffset - (defaultHeight - currentHeight) * 0.5f);
            capsule.height = currentHeight;
        }
    }
}
