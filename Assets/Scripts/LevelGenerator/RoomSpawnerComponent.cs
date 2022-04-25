using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawnerComponent : MonoBehaviour
{
    [SerializeField]
    protected Transform[] spawnPoints;
    protected int spawnCount;
    public void Spawn()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {

        }   
    }
}
