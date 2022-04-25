using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomActor : Actor
{

    protected Vector2Int id;
    [SerializeField]
    protected float size;
    protected LevelGenerator.RoomDoorEnum open;
    [SerializeField]
    protected RoomPartComponent[] parts;
    protected SpawnListScriptableObject spawnList;
    protected bool wasSpawed;
    public float Size => size;
    public Vector2Int Id { get => id; set => id = value; }
    public void SetSpawnList(SpawnListScriptableObject spawnList)
    {
        this.spawnList = spawnList;
    }
    [ContextMenu("Spawn")]
    public void Spawn()
    {
        int spawnCount = 0;
        if (!wasSpawed)
        {
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].IsOpen)
                    parts[i].Spawn(spawnList, ref spawnCount);
            }
            wasSpawed = true;
        }
        
    }
    public void OpenDoors(LevelGenerator.RoomDoorEnum open)
    {
        this.open = open;
        if ((open & LevelGenerator.RoomDoorEnum.right).Equals(LevelGenerator.RoomDoorEnum.right))
            parts[0].Open();
        else
            parts[0].Close();
        if ((open & LevelGenerator.RoomDoorEnum.up).Equals(LevelGenerator.RoomDoorEnum.up))
            parts[1].Open();
        else
            parts[1].Close();
        if ((open & LevelGenerator.RoomDoorEnum.left).Equals(LevelGenerator.RoomDoorEnum.left))
            parts[2].Open();
        else
            parts[2].Close();
        if ((open & LevelGenerator.RoomDoorEnum.down).Equals(LevelGenerator.RoomDoorEnum.down))
            parts[3].Open();
        else
            parts[3].Close();
    }
    
}
