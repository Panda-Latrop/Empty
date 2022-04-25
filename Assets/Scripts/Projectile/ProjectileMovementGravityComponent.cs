using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovementGravityComponent : ProjectileMovementComponent
{
    [SerializeField]
    protected float gravityScale = 1.0f;
    protected override void Movement()
    {
        rigidbody.MoveRotation(angle);
        rigidbody.velocity = velocity;
        velocity += Physics.gravity * gravityScale * Time.fixedDeltaTime;
    }
}
