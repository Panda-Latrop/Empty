using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathComponent : MonoBehaviour
{
    protected Pawn owner;
    protected SkeletalMeshComponent skeletal;
    [SerializeField]
    protected RagdollActor ragdoll;
    public void SetOwner(Pawn owner)
    {
        this.owner = owner;
    }
    public void SetSkeletalMesh(SkeletalMeshComponent skeletal)
    {
        this.skeletal = skeletal;
    }
    public void Death(DamageStruct ds, RaycastHit raycastHit)
    {
        
        RagdollActor ragdoll = GameInstance.Instance.PoolManager.Pop(this.ragdoll) as RagdollActor;
        //RagdollActor ragdoll = GameInstance.Instance.PoolManager.Pop(this.ragdoll) as RagdollActor;
        ragdoll.transform.position = transform.position;
        ragdoll.transform.rotation = transform.rotation;
        ragdoll.CopySkeletal(skeletal);
        ragdoll.AddForce(ds.bone, (ds.direction + Vector3.up) * ds.power, raycastHit.point,ForceMode.Impulse);
        GameInstance.Instance.PoolManager.Push(owner);
        
    }
}
