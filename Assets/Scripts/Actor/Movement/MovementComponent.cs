using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum PathState
{
    none,
    request,
    has,
}
public class MovementComponent : MonoBehaviour, ISaveableComponent
{
    protected new Rigidbody rigidbody;
    protected new Collider collider;
    protected float defaultHeight;
    [SerializeField]
    protected float maxSpeed = 4.5f, speedMultiply = 1.0f, jumpPower = 5.0f;
    protected Vector3 direction, velocity;
    protected bool hasMove,hasJump;

    protected bool grounded, canSlope;
    [SerializeField]
    protected float maxSlopeDot = 0.707f;
    protected Quaternion slopeQuaternion;
    RaycastHit[] slopeHits = new RaycastHit[8];
    int count;
    [SerializeField]
    protected float maxStepPower = 4.0f;
    protected float timeToJump = 0.2f;
    protected float nextJump;

    private System.Action OnJump;
    private System.Action OnGround;


    public bool IsMove { 
        get 
        {
            //Vector3 velocity = rigidbody.velocity; 
            //velocity.y = 0.0f;
            //Debug.Log(velocity.sqrMagnitude + " " + (maxSpeed * speedMultiply * 0.5f));
            // return rigidbody.velocity.sqrMagnitude >= maxSpeed*speedMultiply*0.5f; 
            //return rigidbody.velocity.sqrMagnitude > 0.5f || hasMove;
            return hasMove;
        }
    }
    public bool Grounded => grounded;
    public float SpeedMultiply { get => speedMultiply; set => speedMultiply = value; }
    public Vector3 Direction => direction;

    public void SetRigidbody(Rigidbody rigidbody)
    {
        this.rigidbody = rigidbody;
    }
    public void SetCollider(Collider collider)
    {
        this.collider = collider;
        defaultHeight = collider.bounds.size.y;
    }
    public void Teleport(Vector3 position)
    {
        rigidbody.MovePosition(rigidbody.position+position);
    }
    public void Jump(float direction = 1)
    {
        if ((direction < 0 ||direction > 0))
        {
            hasJump = true;           
            this.direction.Set(this.direction.x, direction, this.direction.z);
        }
        else
        {
            hasJump = false;
        }
    }
    public void Move(Vector3 direction, bool moveForward = true)
    {
        
        if (direction.sqrMagnitude > 0)
        {
            direction.y = 0;
            direction.Normalize();
            hasMove = true;
            if (moveForward)
                direction = transform.root.localRotation * direction;
            this.direction.Set(direction.x, this.direction.y, direction.z);
        }
        else
        {
            hasMove = false;
        }
    }
    


    protected bool GroundCheck()
    {
        //Vector3 center = transform.position;
        Vector3 center = collider.bounds.center;
        Vector3 extents = collider.bounds.extents;
        //center.y += extents.y;
        extents.x *= 0.7f;
        extents.y *= 1.2f;
        extents.z *= 0.7f;
        //extents.y = 0.15f;
        //Debug.DrawLine(center,center+direction)
        bool grounded = Physics.CheckBox(center, extents, Quaternion.identity, (1 << 6) | (1 << 7), QueryTriggerInteraction.Ignore);
        if (!this.grounded && grounded)
            CallOnGround();
        return this.grounded =grounded;
    }
    protected bool StepCheck(Vector3 direction)
    {

        RaycastHit lowerHit;
        //Vector3 center = transform.position;
        Vector3 center = collider.bounds.center;
        Vector3 extents = collider.bounds.extents;
        Vector3 lower, upper;
        lower = transform.position;
        upper = lower;
        upper.y += defaultHeight * 0.2f;
        float length = extents.x*1.5f;

        Debug.DrawLine(lower, lower + direction * length);
        Debug.DrawLine(upper, upper + direction * length);

        if (Physics.Raycast(lower, direction, out lowerHit, length, (1 << 6), QueryTriggerInteraction.Ignore))
            if (lowerHit.normal.y <= 0.0f && !Physics.Raycast(upper, direction, length, (1 << 6), QueryTriggerInteraction.Ignore))
                return true;
        return false;
    }
    protected bool SlopeCheck()
    {
        //Vector3 center = transform.position;      
        Vector3 center = collider.bounds.center;
        Vector3 extents = collider.bounds.extents;
        float length = extents.y;
        extents *= 0.8f;
        if (Physics.SphereCast(center, extents.x, Vector3.down,out slopeHits[0], length, (1 << 6) | (1 << 7), QueryTriggerInteraction.Ignore))
        {
            count = 1;
            RaycastHit slopeNormal = slopeHits[0];
            //for (int i = 1; i < count; i++)
            //{
            //    if (Mathf.Abs(slopeNormal.normal.y) < Mathf.Abs(slopeHits[i].normal.y))
            //        slopeNormal = slopeHits[i];
            //}
            if (canSlope = (slopeNormal.normal.y >= maxSlopeDot))
            {
                slopeQuaternion = Quaternion.FromToRotation(Vector3.up, slopeNormal.normal);
            }
        }
        else
        {
            count = 0;
            canSlope = false;
        }
        return canSlope;
    }

    protected void FixedUpdate()
    {

        if (Time.time >= nextJump)
        {
            GroundCheck();
            SlopeCheck();
            canSlope = canSlope && grounded;

            if (hasJump && grounded && canSlope)
            {
                velocity.Set(rigidbody.velocity.x, direction.y * jumpPower, rigidbody.velocity.z);
                rigidbody.velocity = velocity;
                CallOnJump();
                grounded = false;
                canSlope = false;
                nextJump = timeToJump + Time.time;
            }
        }    
        direction.y = 0.0f;
        Vector3 move = Vector3.zero;
        if (hasMove)
            move = direction * maxSpeed * speedMultiply;
        if (grounded && canSlope)
        {
            Vector3 stepVelocity = Vector3.zero;
            if (StepCheck(direction) && direction.sqrMagnitude > 0.0f)
                stepVelocity = Vector3.up * maxStepPower;
            velocity = move + Physics.gravity* 4.0f * Time.fixedDeltaTime;
            velocity = slopeQuaternion * velocity;
            rigidbody.velocity = velocity + stepVelocity;          
        }
        else
        if (!grounded && !canSlope)
        {
            velocity = move;
            velocity.y = rigidbody.velocity.y;
            rigidbody.velocity = velocity;
            
        }         
        if (canSlope)
            rigidbody.AddForce(slopeQuaternion * Physics.gravity,ForceMode.Acceleration);//* rigidbody.mass);
        else
            rigidbody.AddForce(Physics.gravity, ForceMode.Acceleration);// * rigidbody.mass);    
        hasJump = false;
        hasMove = false;
        //direction = Vector3.zero;
    }
   
    
    
    public virtual JSONObject Save(JSONObject jsonObject)
    {
        jsonObject.Add("enabled", enabled);
        jsonObject.Add("speedMultiply", new JSONNumber(speedMultiply));
        JSONArray velocityJArray = new JSONArray();
        {
            velocityJArray.Add(new JSONNumber(velocity.x));
            velocityJArray.Add(new JSONNumber(velocity.y));
            velocityJArray.Add(new JSONNumber(velocity.z));
        }
        jsonObject.Add("velocity", velocityJArray);
        JSONArray directionJArray = new JSONArray();
        {
            directionJArray.Add(new JSONNumber(direction.x));
            directionJArray.Add(new JSONNumber(direction.y));
            directionJArray.Add(new JSONNumber(direction.z));
        }
        jsonObject.Add("direction", directionJArray);
        return jsonObject;
    }
    public virtual JSONObject Load(JSONObject jsonObject)
    {
        enabled = jsonObject["enabled"].AsBool;     
        speedMultiply = jsonObject["speedMultiply"].AsFloat;   
        JSONArray velocityJArray = jsonObject["velocity"].AsArray;
        velocity.Set(velocityJArray[0].AsFloat, velocityJArray[1].AsFloat, velocityJArray[2].AsFloat);
        JSONArray directionJArray = jsonObject["direction"].AsArray;
        direction.Set(directionJArray[0].AsFloat, directionJArray[1].AsFloat, directionJArray[2].AsFloat);
        return jsonObject;
    }
    public void CallOnJump()
    {
        OnJump?.Invoke();
    }
    public void BindOnJump(System.Action action)
    {
        OnJump += action;
    }
    public void UnbindOnJump(System.Action action)
    {
        OnJump -= action;
    }
    public void ClearOnJump()
    {
        OnJump = null;
    }
    public void CallOnGround()
    {
        OnGround?.Invoke();
    }
    public void BindOnGround(System.Action action)
    {
        OnGround += action;
    }
    public void UnbindOnGround(System.Action action)
    {
        OnGround -= action;
    }
    public void ClearOnGround()
    {
        OnGround = null;
    }
    protected void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            Vector3 center = collider.bounds.center;
            for (int i = 0; i < count; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawCube(slopeHits[i].point, Vector3.one * 0.1f);
                Gizmos.color = Color.green;
                Gizmos.DrawLine(slopeHits[i].point, slopeHits[i].point + slopeHits[i].normal);
                Gizmos.color = Color.magenta;
                Vector3 p = slopeHits[i].normal;
                p.y *= -1.0f;
                Gizmos.DrawLine(slopeHits[i].point, slopeHits[i].point + p);
            }

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(center - slopeQuaternion * transform.forward, center + slopeQuaternion * transform.forward);
        }
    }
}
