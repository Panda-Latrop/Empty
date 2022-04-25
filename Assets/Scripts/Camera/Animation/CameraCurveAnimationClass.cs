using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class CameraCurveAnimationClass
{
    protected bool isFinished = true;

    [SerializeField]
    protected AnimationCurveScriptableObject curve;
    protected Vector3 vector;
    protected bool blendState;
    protected float time,blend;
    protected float scale = 1.0f, speed = 1.0f;
    protected Vector3 lerp;

    public float Time { get => time; set => time = Mathf.Clamp01(value); }
    public Vector3 Vector => vector;
    public float Scale { get => scale; set => scale = value; }
    public float Speed { get => speed; set => speed = value; }
    public bool IsFinished => isFinished;

    public void SetCurve(AnimationCurveScriptableObject curve)
    {
        this.curve = curve;
        isFinished = false;
    }

    public virtual void IncreaseTime()
    {
        if (time < 1.0f)
        {
            time += curve.Speed * speed * UnityEngine.Time.deltaTime;
            if (time >= 1.0f)
            {
                time = 1.0f;
                isFinished = true;
            }
            else
            {
                isFinished = false;
            }

        }
    }
    public virtual void DecreaseTime()
    {
        if (time > 0.0f)
        {
            time -= curve.Speed * speed * UnityEngine.Time.deltaTime;
            if (time <= 0.0f)
                time = 0.0f;
        }
    }
    public virtual void BlendIn()
    {
        blend += curve.BlendIn * UnityEngine.Time.deltaTime;
        if (blend >= 1.0f)
            blend = 1.0f;
    }
    public virtual void BlendOut()
    {
        blend -= curve.BlendOut * UnityEngine.Time.deltaTime;
        if (blend <= 0.0f)
            blend = 0.0f;
    }

    public virtual void Evaluate()
    {

        if (blend < 1.0f)
        {
            if (blendState == true)
                lerp = vector;
            blendState = false;
            vector = curve.Evaluate(time) * scale;
            vector.x *= -1.0f;
            vector = Vector3.Lerp(lerp, vector, blend);
            BlendIn();
        }
        else
        {
            vector = curve.Evaluate(time) * scale;
            vector.x *= -1.0f;
        }
        IncreaseTime();
    }
    public virtual void Fading()
    {
        if (blend > 0.0f)
        {
            if (blendState == false)
                lerp = vector;
            blendState = true;
            vector = Vector3.Lerp(lerp, Vector3.zero, (1.0f - blend));
            BlendOut();
        }
        else
        {
            vector = Vector3.zero;
        }
        DecreaseTime();
    }
    public virtual void Stop()
    {
        blendState = true;
        lerp = vector = Vector3.zero;
        time = 0.0f;
        scale = 1.0f;
        blend = 0.0f;
        isFinished = true;
    }
}
