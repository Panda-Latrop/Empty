using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicImpactActor : DynamicActor
{
    [SerializeField]
    protected ParticleSystem particle;
    [SerializeField]
    protected new AudioSource audio;

    public override void OnPop()
    {
        base.OnPop();
        particle.Play(true);
        audio.Play();
    }
    public override void OnPush()
    {
        base.OnPush();
        particle.Stop(true);
        audio.Stop();
    }
}
