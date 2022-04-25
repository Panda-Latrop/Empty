using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerNextLevelActor : TriggerActor
{
    protected bool isActive;
    [SerializeField]
    public string nextLevel;
    protected float timeToNextLevel = 2.0f;
    protected float nextTime;
    protected override void Execute(Collider other)
    {
        base.Execute(other);
        isActive = true;
        enabled = true;
        nextTime = Time.time + timeToNextLevel;
    }

    protected void LateUpdate()
    {
        //Debug.Log(isActive +" "+ (Time.time >= nextTime));
        if (isActive && Time.time >= nextTime)
        {
            GameInstance.Instance.LoadScene(nextLevel.ToLower(), 0);
            enabled = false;
            isActive = false;
        }
    }

}
