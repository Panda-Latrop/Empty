using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GameState : Actor
{
    protected Dictionary<int, int> questsStates = new Dictionary<int, int>();
    public virtual void PlayerStart(PlayerStart playerStart, PlayerController playerController)
    {
        Vector3 position;
        Quaternion rotation;
        playerStart.GetPoint(0, out position, out rotation);
        playerController.ControlledPawn.SetTransform(position, rotation);
    }
    protected Dictionary<GameObject, int> scoreTable = new Dictionary<GameObject, int>();
    [ContextMenu("Log Score")]
    private void Debug_LogScore()
    {
        StringBuilder strb = new StringBuilder();
        foreach (var item in scoreTable)
        {
            strb.Append(item.Key.name).Append(" : ").Append(item.Value).Append('\n');
        }
        Debug.Log(strb.ToString());
    }
    [ContextMenu("Add Score")]
    private void Debug_AddScore()
    {
        AddScore(GameInstance.Instance.PlayerController.ControlledPawn.gameObject, 1000);
    }
    public virtual void PawnDeath(Pawn actor, DamageStruct ds, RaycastHit raycastHit) 
    {

    }
    public virtual void PawnHurt(Pawn actor, DamageStruct ds, RaycastHit raycastHit)
    {

    }
    public virtual int GetScore(GameObject who)
    {
        int score = 0;
        if (!scoreTable.TryGetValue(who, out score))
            scoreTable.Add(who, 0);
        return score;
    }
    public virtual void AddScore(GameObject who, int score) 
    {      
        int add = GetScore(who) + score;
        scoreTable[who] = add;
    }
    public virtual void RemoveScore(GameObject who, int score) {
        int remove = GetScore(who) - score;
        scoreTable[who] = remove;
    }
    public virtual void GameOver() {}
    public void SetQuestState(int quest,int state)
    {
        if (questsStates.ContainsKey(quest))
        {
            questsStates[quest] = state;
        }else
        {
            questsStates.Add(quest, state);
        }
    }
    public int GetQuestState(int quest)
    {


        int state = 0;
        if (questsStates.TryGetValue(quest, out state))
        {
            return state;
        }
        else
        {
            state = 0;
            questsStates.Add(quest, state);
            return state;
        }
        
    }
}
