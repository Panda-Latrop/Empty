using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletalMeshComponent : MonoBehaviour
{
    [SerializeField]
    protected Transform[] bones;
    protected Dictionary<string, Transform> boneMap = new Dictionary<string, Transform>();


    protected void Awake()
    {
        for (int i = 0; i < bones.Length; i++)
        {
            boneMap.Add(bones[i].name, bones[i]);
        }
        bones = null;
    }
    public bool GetBone(string name, out Transform bone)
    {
       return boneMap.TryGetValue(name,out  bone);
    }
}
