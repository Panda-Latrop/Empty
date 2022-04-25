using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : ControllerBase
{
    public bool gameInput = true;
    protected GameState gameState;

    protected void Start()
    {
        gameState = GameInstance.Instance.GameState;
    }
    protected override void OnDeath(DamageStruct ds, RaycastHit raycastHit)
    {
        gameState.PawnDeath(controlledPawn, ds, raycastHit);
    }
    public virtual void GameOver()
    {

    }
}
