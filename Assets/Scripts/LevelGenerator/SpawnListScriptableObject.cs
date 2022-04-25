using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spawn List", menuName = "Spawn/Spawn List", order = 2)]
public class SpawnListScriptableObject : ScriptableObject
{
    [SerializeField]
    protected List<Pawn> pawns = new List<Pawn>();
    [SerializeField]
    protected List<float> chance = new List<float>();
    [SerializeField]
    protected int spawnCount = 10;

    public int SpawnCount => spawnCount;
    public Pawn GetPawn()
    {
        float rand = Random.value;
        if (chance.Count >= 2)
        {
            if (rand >= 0 && rand <= chance[0])
            {
                return pawns[0];
            }
                
            for (int i = 1; i < chance.Count; i++)
            {
                if (rand > chance[i - 1] && rand <= chance[i])
                {
                    return pawns[i];
                }
                    
            }
        }
        return pawns[0];     
    }
}
