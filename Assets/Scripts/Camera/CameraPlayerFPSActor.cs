using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPlayerFPSActor : CameraActor
{
    [SerializeField]
    protected new CameraAnimationComponent animation;
    [SerializeField]
    protected CameraFadeComponent fade;

    public CameraFadeComponent Fade => fade;
    public CameraAnimationComponent Animation => animation;
    protected bool isDead = false;
    [SerializeField]
    protected Vector3 deathPosition;
    protected Vector3 defaultPosition;

    protected void Start()
    {
        defaultPosition = transform.localPosition;
    }
    public void PlayJump()
    {
        animation.PlayJump();
    }
    public void PlayGround()
    {
        animation.PlayGround();
    }
    public void PlayRecoil(AnimationCurveScriptableObject curve)
    {
        LookRotate(animation.PopRecoilVector());
        animation.PlayRecoil(curve);
    }
    public void PlayDeath()
    {
        transform.localPosition = deathPosition;
        isDead = true;
    }
    public void StopDeath()
    {
        transform.localPosition = defaultPosition;
        isDead = false;
    }
    protected override void Update()
    {
        if (isDead)
        {
            animation.AnimateDeath();
            Vector3 rotation = Utility.ClampRotation(lookRotation + animation.GetDeathVector(), -90.0f, 90.0f);
            transform.localRotation = Quaternion.Euler(rotation);
        }
        else
        {
            if (animation.AnimateRecoil())
                LookRotate(animation.PopRecoilVector());
            animation.Animate();

            Vector3 rotation = Utility.ClampRotation(lookRotation + animation.Rotation(), -90.0f, 90.0f);
            transform.localRotation = Quaternion.Euler(rotation);
        }
    }
}
