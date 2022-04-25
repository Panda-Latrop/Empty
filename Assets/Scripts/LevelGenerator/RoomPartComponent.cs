using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPartComponent : MonoBehaviour
{
    protected bool isOpen = true;
    [SerializeField]
    protected GameObject[] toHide,toShow;
    [SerializeField]
    protected Transform[] spawnPoints;
    [SerializeField]
    protected float spawnChance = 1.0f;
    public bool IsOpen => isOpen;
    public void Open()
    {
        for (int i = 0; i < toHide.Length; i++)
        {
            toHide[i].SetActive(false);
        }
        for (int i = 0; i < toShow.Length; i++)
        {
            toShow[i].SetActive(true);
        }
        isOpen = true;
    }
    public void Close()
    {
        for (int i = 0; i < toHide.Length; i++)
        {
            toHide[i].SetActive(true);
        }
        for (int i = 0; i < toShow.Length; i++)
        {
            toShow[i].SetActive(false);
        }
        isOpen = false;
    }
    public void Spawn(SpawnListScriptableObject spawnList, ref int spawnCount)
    {
        //Debug.Log(spawnCount + " " + (spawnCount < spawnList.SpawnCount));
        if (spawnCount < spawnList.SpawnCount)
        {
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                if (Random.value <= spawnChance)
                {
                    IPoolObject ipo = GameInstance.Instance.PoolManager.Pop(spawnList.GetPawn());
                    ipo.SetPosition(spawnPoints[i].position);
                    ipo.SetRotation(Quaternion.AngleAxis(Random.Range(-180.0f, 180.0f), Vector3.up));
                    spawnCount++;
                    if (spawnCount >= spawnList.SpawnCount)
                        return;
                }
            }
        }
    }
    protected void OnDrawGizmosSelected()
    {
        if (spawnPoints != null)
        {
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawSphere(spawnPoints[i].position + Vector3.up* 0.5f, 0.5f);
            }
        }
    }
}
