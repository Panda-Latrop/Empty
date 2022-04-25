using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBattleState : GameState
{
    [System.Serializable]
    protected struct PawnScorePricePairStruct
    {
       public Pawn pawn;
       public int price;
    }
    [SerializeField]
    protected PawnScorePricePairStruct[] pawnScorePricePairs;
    [SerializeField]
    protected int defaultScore = 100,hurtScore = 10;

    protected Dictionary<string, int> pawnScorePriceMap = new Dictionary<string, int>();

    protected void Awake()
    {
        for (int i = 0; i < pawnScorePricePairs.Length; i++)
        {
            var pair = pawnScorePricePairs[i];
            pawnScorePriceMap.Add(pair.pawn.Specifier, pair.price);
        }
    }

    public override void PawnDeath(Pawn actor, DamageStruct ds, RaycastHit raycastHit)
    {      
        int score = defaultScore;
        pawnScorePriceMap.TryGetValue(actor.Specifier, out score);
        AddScore(ds.causer, score);     
    }
    public override void PawnHurt(Pawn actor, DamageStruct ds, RaycastHit raycastHit)
    {
        int score = hurtScore;
        AddScore(ds.causer, score);
    }
}
