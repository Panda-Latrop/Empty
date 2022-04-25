using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDoorQuestActor : TriggerActor
{
    [SerializeField]
    protected int quest;
    [SerializeField]
    protected SimpleDoorActor door;

    protected override void Execute(Collider other)
    {
        //base.Execute(other);
        if(GameInstance.Instance.GameState.GetQuestState(quest) == 0)
        door.Open();
    }
}
