using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CameraCurveTwoDirectAnimationClass : CameraCurveAnimationClass
{
    public override void Evaluate()
    {
        vector = curve.Evaluate(time) * scale;
        vector.x *= -1.0f;
        IncreaseTime();
    }
    public override void Fading()
    {
        vector = curve.Evaluate(time) * scale;
        vector.x *= -1.0f;
        DecreaseTime();
    }
}
