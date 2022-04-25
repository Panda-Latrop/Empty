using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchComponent : MonoBehaviour, ISaveableComponent
{
    protected bool isCrouch;
    protected CapsuleCollider capsule;
    protected MovementComponent movement;
    [SerializeField]
    protected float crouchPercent = 0.5f, riseSpeed = 5.0f, speedMultiply = 0.5f;
    protected float defaultHeight, defaultOffset, crouchHeight, currentHeight = -1.0f;

    public bool IsCrouch => isCrouch;// || !currentHeight.Equals(defaultHeight);
    public float SpeedMultiply => speedMultiply;
    public float Percent => crouchPercent;
    public float Speed => riseSpeed;
    public void SetCapsule(CapsuleCollider capsule)
    {
        this.capsule = capsule;
        defaultHeight = capsule.height;
        defaultOffset = capsule.center.y;
        crouchHeight = defaultHeight * crouchPercent;
        if(currentHeight < 0)
        currentHeight = defaultHeight;
    }
    public void SetMovement(MovementComponent movement)
    {
        this.movement = movement;
    }
    public void Crouch(bool crouch)
    {
        if (crouch)
        {
            if (!isCrouch)
            {
                isCrouch = true;
                enabled = true;
            }

        }
        else
        {
            if (isCrouch)
            {
                isCrouch = false;
            }
        }
    }

    protected virtual void SitDown()
    {
        currentHeight = crouchHeight;
        capsule.center = Vector3.up * (defaultOffset - (defaultHeight - currentHeight) * 0.5f);
        capsule.height = currentHeight;
        if (!movement.Grounded)
            movement.Teleport(Vector3.up * (defaultHeight - crouchHeight));
    }
    protected virtual void StandUp()
    {
        RaycastHit hit;
        Vector3 center = capsule.bounds.center;
        Vector3 extents = capsule.bounds.extents;
        center.y += defaultOffset - capsule.center.y;
        if (!Physics.SphereCast(center, extents.x, Vector3.up, out hit, defaultHeight * 0.5f - extents.x, (1 << 6), QueryTriggerInteraction.Ignore))
        {
            currentHeight = defaultHeight;
            capsule.center = capsule.center = Vector3.up * (defaultOffset - (defaultHeight - currentHeight) * 0.5f);
            capsule.height = currentHeight;
            if (!movement.Grounded)
                movement.Teleport(Vector3.down * (defaultHeight - crouchHeight));
            enabled = false;
        }
    }
    protected virtual void FixedUpdate()
    {
        if (isCrouch)
        {
            if (currentHeight > crouchHeight)
            {
                SitDown();
            }
        }
        else
        {
            if (currentHeight < defaultHeight)
            {
                StandUp();
            }
            else
            {
                enabled = false;
            }
        }
    }

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.green;
    //    Vector3 center = capsule.bounds.center;
    //    Vector3 extents = capsule.bounds.extents;
    //    center.y += defaultOffset - capsule.center.y;
    //    Gizmos.DrawWireSphere(center, extents.x);
    //    Gizmos.DrawWireSphere(center + Vector3.up * (defaultHeight * 0.5f - extents.x), extents.x);
    //}

    public JSONObject Save(JSONObject jsonObject)
    {
        throw new System.NotImplementedException();
    }

    public JSONObject Load(JSONObject jsonObject)
    {
        throw new System.NotImplementedException();
    }
}
