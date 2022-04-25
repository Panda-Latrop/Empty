using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollActor : Actor , IPoolObject
{
    [SerializeField]
    protected Transform root;
    [SerializeField]
    protected Rigidbody[] bones;
    protected Dictionary<string, Rigidbody> boneMap = new Dictionary<string, Rigidbody>();
    [SerializeField]
    protected float timeToPush = 10.0f;
    protected float nextPush;
    [SerializeField]
    protected bool playSound;
    [SerializeField]
    protected AudioSource source;
    [SerializeField]
    protected AudioCueScriptableObject sound;

    public string PoolTag => this.GetType().Name + specifier;

    protected void Awake()
    {
        for (int i = 0; i < bones.Length; i++)
        {
            boneMap.Add(bones[i].name, bones[i]);
        }
        bones = null;
    }
    protected void Start()
    {
        nextPush = Time.time + timeToPush;
    }
    public void CopySkeletal(SkeletalMeshComponent skeletal)
    {
        Transform t;
        if (skeletal.GetBone(root.name, out t))
        {
            root.transform.localPosition = t.localPosition;
            root.transform.localRotation = t.localRotation;
        }
        foreach (var bone in boneMap)
        {

            if (skeletal.GetBone(bone.Key, out t))
            {
                bone.Value.transform.localPosition = t.localPosition;
                bone.Value.transform.localRotation = t.localRotation;
            }

        }
    }

    public void AddForce(string name,Vector3 force, Vector3 position,ForceMode forceMode)
    {
        Rigidbody bone;
        float scale = 1;
        if (name.Equals("head"))
            scale = 1.25f;
        if (boneMap.TryGetValue(name,out bone))
            bone.AddForceAtPosition(force* scale, position, forceMode);
    }
    protected void LateUpdate()
    {
        if (Time.time >= nextPush)
            GameInstance.Instance.PoolManager.Push(this);
    }
    public void OnPush()
    {
        gameObject.SetActive(false);
    }

    public void OnPop()
    {
        gameObject.SetActive(true);
        nextPush = Time.time + timeToPush;
        if(playSound)
        {
            source.clip = sound;
            source.Play();
        }    
    }
}
