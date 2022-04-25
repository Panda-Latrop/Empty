using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartRoomActor : RoomActor
{
    public SimpleDoorActor door;
    public TriggerNextLevelActor trigger;
    public void DoorState(bool open)
    {
        if(open)
        door.Open();
        else
        door.Close();
    }
    
}
