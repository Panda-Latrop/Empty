using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnimationComponent : MonoBehaviour
{

    protected MovementComponent movement;
    protected SprintComponent sprint;
    protected CrouchComponent crouch;
    protected SlideComponent slide;


    [SerializeField]
    protected CameraCurveLoopAnimationClass movementAnimation;
    [SerializeField]
    protected CameraCurveAnimationClass slideAnimation, jumpAnimation, groundAnimation, recoilAnimation;
    [SerializeField]
    protected CameraCurveLoopAnimationClass deathAnimation;
    
    [SerializeField]
    protected float timeToPlayGround = 1.0f;
    protected bool hasLastGroundTime;
    protected float lastGroundTime;

    public void SetMovement(MovementComponent movement)
    {
        this.movement = movement;
    }
    public void SetSprint(SprintComponent sprint)
    {
        this.sprint = sprint;
    }
    public void SetCrouch(CrouchComponent crouch)
    {
        this.crouch = crouch;
        //crouchAnimation.Scale = this.crouch.Percent;
        //crouchAnimation.Speed = this.crouch.Speed;
    }
    public void SetSlide(SlideComponent slide)
    {
        this.slide = slide;
    }
    public void PlayJump()
    {
        jumpAnimation.Evaluate();
    }
    public void PlayGround()
    {
        if(Time.time - lastGroundTime >= timeToPlayGround)
        {
            groundAnimation.Evaluate();
            hasLastGroundTime = false;
        }

    }
    public void PlayRecoil(AnimationCurveScriptableObject curve)
    {
        recoilAnimation.Stop();
        recoilAnimation.SetCurve(curve);
        recoilAnimation.Evaluate();
    }
    public void PlayDeath()
    {
        deathAnimation.Evaluate();
    }
    public void StopDeath()
    {
        deathAnimation.Stop();
    }
    public bool AnimateDeath()
    {
        deathAnimation.Evaluate();
        return false;
    }
    public Vector3 GetDeathVector()
    {
        return deathAnimation.Vector;
    }
    public bool AnimateRecoil()
    {
        if (!recoilAnimation.IsFinished)
        {
            recoilAnimation.Evaluate();
            if(recoilAnimation.IsFinished)
            return true;
        }
        return false;
    }
    public Vector3 PopRecoilVector()
    {
        Vector3 v = recoilAnimation.Vector;
        recoilAnimation.Stop();
        return v;
    }
    public void Animate()
    {
        if (slide.IsSlide && movement.Grounded)
        {
            slideAnimation.Evaluate();
        }
        else
        {
            slideAnimation.Fading();
        }
        if (movement.IsMove && movement.Grounded && !slide.IsSlide)
        {
            movementAnimation.Speed = movementAnimation.Scale = movement.SpeedMultiply;
            movementAnimation.Evaluate();
        }          
        else
        {
            movementAnimation.Speed = movementAnimation.Scale = 1.0f;
            movementAnimation.Fading();
        }

        if(!movement.Grounded && !hasLastGroundTime)
        {
            hasLastGroundTime = true;
            lastGroundTime = Time.time;
        }

        if (!jumpAnimation.IsFinished)
            jumpAnimation.Evaluate();
        else
            jumpAnimation.Fading();

        if (!groundAnimation.IsFinished)
            groundAnimation.Evaluate();
        else
            groundAnimation.Fading();


        //if (crouch.IsCrouch)
        //{
        //    if (!movement.Grounded)
        //        crouchAnimation.Time = 1.0f;
        //    crouchAnimation.Evaluate();
        //}
        //else
        //{
        //    if (!movement.Grounded)
        //        crouchAnimation.Time = 0.0f;
        //    crouchAnimation.Fading();
        //}
    }

    public Vector3 Rotation()
    {
        Vector3 vector = Vector3.zero;
        vector += movementAnimation.Vector;
        vector += slideAnimation.Vector;
        vector += jumpAnimation.Vector;
        vector += groundAnimation.Vector;
        vector += recoilAnimation.Vector;
        return vector;
    }
    //public Vector3 Position()
    //{

    //    Vector3 vector = Vector3.zero;
    //    vector += crouchAnimation.Vector;
    //    return vector;
    //}
}
