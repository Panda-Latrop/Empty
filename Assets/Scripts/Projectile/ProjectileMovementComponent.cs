using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovementComponent : MonoBehaviour, ISaveableComponent
{
    protected new Rigidbody rigidbody;
    protected float speed;
    protected bool hasInput;
    protected Vector3 velocity;
    protected Quaternion angle;
    public float Speed { get => speed; set => speed = value; }
    public void SetRigidbody(Rigidbody rigidbody)
    {
        this.rigidbody = rigidbody;
    }
    public virtual void SetPosition(Vector3 position)
    {
        rigidbody.MovePosition(position);
    }
    public virtual void Move(Vector3 direction)
    {
        velocity = direction * speed;
        angle = Quaternion.LookRotation(direction,Vector3.up);
        hasInput = true;
    }
    public void Stop()
    {
        rigidbody.velocity = Vector3.zero;
        hasInput = false;
    }
    protected virtual void Movement()
    {
        if (hasInput)
        {
            rigidbody.velocity = velocity;
            rigidbody.MoveRotation(angle);
            hasInput = false;
        }
    }
    protected  void FixedUpdate()
    {
        Movement();
    }

    public virtual JSONObject Save( JSONObject jsonObject)
    {
        return jsonObject;
    }
    public virtual JSONObject Load( JSONObject jsonObject)
    {
        return jsonObject;
    }

}
