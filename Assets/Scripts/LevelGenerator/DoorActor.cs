using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorActor : TriggerActor
{
    [SerializeField]
    protected Animator animator;
    [SerializeField]
    protected AudioSource source;
    protected int toOpenHash = Animator.StringToHash("ToOpen"),
                  toCloseHash = Animator.StringToHash("ToClose");

    protected RoomActor roomA, roomB;
    
    public void SetRooms(RoomActor roomA, RoomActor roomB)
    {
        this.roomA = roomA;
        this.roomB = roomB;
    }

    public virtual void Open(int room)
    {
        animator.Play(toOpenHash);
        source.Play();
        if (room == 0)
            roomA.Spawn();
        else
            roomB.Spawn();
    }
    public virtual void Close()
    {
        animator.Play(toCloseHash);
        source.Play();
    }
    protected override void Execute(Collider other)
    {
        if ((other.transform.position - roomA.transform.position).sqrMagnitude < (other.transform.position - roomB.transform.position).sqrMagnitude)
            Open(1);
        else
            Open(0);
    }
}
