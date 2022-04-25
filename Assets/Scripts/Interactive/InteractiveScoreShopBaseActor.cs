using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractiveScoreShopBaseActor : InteractiveActor
{
    [SerializeField]
    protected int cost;
    protected GameState state;
    protected void Start()
    {
        state = GameInstance.Instance.GameState;
    }
    public override void Intercat(Character by)
    {
        GameObject who = by.gameObject;
        int score = state.GetScore(who);
        if(score >= cost && Buy(by))
        {
            state.RemoveScore(who, cost);
        }
    }
    protected abstract bool Buy(Character by);
}
