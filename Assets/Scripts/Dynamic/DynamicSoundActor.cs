using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicSoundActor : DynamicActor
{
    [SerializeField]
    protected new AudioSource audio;

    public override void OnPop()
    {
        base.OnPop();
        audio.Play();
    }
    public override void OnPush()
    {
        base.OnPush();
        audio.Stop();
    }
}

