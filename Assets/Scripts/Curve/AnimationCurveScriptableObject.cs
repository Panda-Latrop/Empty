using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Animation Curve", menuName = "Curves/AnimationCurve", order = 1)]
public class AnimationCurveScriptableObject : ScriptableObject
{
    [SerializeField]
    protected AnimationCurve[] curves = new AnimationCurve[3];
    [SerializeField]
    protected float[] scale = new float[3];
    [SerializeField]
    protected float speed = 1.0f, blendIn = 1.0f, blendOut = 1.0f;

    public float Speed => speed;
    public float BlendIn => blendIn;
    public float BlendOut => blendOut;

    public float Evaluate(int axis, float time)
    {
        return curves[axis].Evaluate(time) * scale[axis];
    }
    public Vector3 Evaluate(float time)
    {
        Vector3 vector = Vector3.zero;
        vector.x = Evaluate(0, time);
        vector.y = Evaluate(1, time);
        vector.z = Evaluate(2, time);
        return vector;
    }

}