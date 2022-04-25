using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CameraCurveLoopAnimationClass : CameraCurveAnimationClass
{
    public override void IncreaseTime()
    {
        if (time < 1.0f)
        {
            time += curve.Speed * scale * UnityEngine.Time.deltaTime;
            if (time >= 1.0f)
                time = time - 1.0f;
        }
    }
}
