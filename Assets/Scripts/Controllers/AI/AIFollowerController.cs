using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFollowerController : AIController
{
    protected Vector3 lastPosition;
    public void Start()
    {
        target = GameInstance.Instance.PlayerController.ControlledPawn;
        hasTarget = true;
    }

    protected virtual void Update()
    {
        Movement();
    }
    protected void Movement()
    {
        Vector3 distance = (target.Center - character.Center);
        Vector3 direction = distance;
        direction.y = 0;
        direction.Normalize();
        float xzm = distance.x * distance.x + distance.z * distance.z;
        float ym = distance.y * distance.y;
        character.LookRotate(direction, 180.0f * Time.deltaTime);
        if (xzm < 10.0f && ym < 4.0f)
        {
            character.Move(distance.normalized, false);
            //Move(direction);
        }
        else
        {
            if (pathState.Equals(PathState.has))
            {
                MoveByPath();
                if (CheckConditionToRebuildPath())
                    CreatePathRequest(target.transform.position, true);
            }
            else if (pathState.Equals(PathState.none))
            {
                CreatePathRequest(target.transform.position);
            }
        }
    }
    protected virtual void LateUpdate()
    {
        lastPosition = transform.position;
    }
}
