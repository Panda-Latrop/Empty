using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraceActor : Actor, IPoolObject
{
    [SerializeField]
    protected ParticleSystem particle;
    [SerializeField]
    protected float timeToPush = 1.0f;
    protected float nextPush ;

    public string PoolTag => GetType().Name+specifier;
    protected void Start()
    {
        nextPush = Time.time + timeToPush;
    }
    public void SetLine(Vector3 start, Vector3 end,Vector3 dir,float distance)
    {
        transform.position = (start + end) * 0.5f;
        transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        ParticleSystem.ShapeModule shape = particle.shape;
        shape.scale = Vector3.right * distance + Vector3.up + Vector3.forward;
        particle.Play();
    }
    public void OnPop()
    {
        gameObject.SetActive(true);
        nextPush = Time.time + timeToPush;
    }
    public void OnPush()
    {
        gameObject.SetActive(false);
    }
    protected void LateUpdate()
    {
        if(Time.time >= nextPush)
            GameInstance.Instance.PoolManager.Push(this);
    }



}
