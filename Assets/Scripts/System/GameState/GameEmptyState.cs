using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEmptyState : GameBattleState
{
   
    
    [SerializeField]
    protected LevelGenerator levelGenerator;
    protected float startTime,timeToCompleteLevel, nextTimeToCompleteLevel;

    public float GetTimer()
    {
        return timeToCompleteLevel - Time.time + startTime;
    }
    public override void PlayerStart(PlayerStart playerStart, PlayerController playerController)
    {
        //Vector3 position;
        //Quaternion rotation;
        //playerStart.GetPoint(0, out position, out rotation);
        //playerController.ControlledPawn.SetTransform(position, rotation);
        levelGenerator = GameObject.FindGameObjectWithTag("Finish").GetComponent<LevelGenerator>();
        levelGenerator.transform.SetParent(null);
        levelGenerator.Generate();
        Transform start = levelGenerator.GetStart().transform;
        playerController.ControlledPawn.SetTransform(start.position, start.rotation);
        timeToCompleteLevel = levelGenerator.TimeToCompleteLevel;
        startTime = Time.time;
        nextTimeToCompleteLevel = startTime + timeToCompleteLevel;
    }
    public override void PawnDeath(Pawn actor, DamageStruct ds, RaycastHit raycastHit)
    {
        int score = defaultScore;
        pawnScorePriceMap.TryGetValue(actor.Specifier, out score);
        Pawn pawn = GameInstance.Instance.PlayerController.ControlledPawn;
        //Debug.Log(pawn.name.Equals(ds.causer.name) + " "+ pawn.name+ " " +);
        if (pawn.name.Equals(ds.causer.name))
            pawn.Health.Heal(score);
    }
    public override void PawnHurt(Pawn actor, DamageStruct ds, RaycastHit raycastHit)
    {
        int score = hurtScore;
        Pawn pawn = GameInstance.Instance.PlayerController.ControlledPawn;
        if (pawn.name.Equals(ds.causer.name))
            pawn.Health.Heal(score);
    }
    protected void LateUpdate()
    {
        if(Time.time >= nextTimeToCompleteLevel)
        {
            GameInstance.Instance.PlayerController.GameOver();
        }
    }
}
