using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : ControllerBase
{
    protected GameState gameState;
    protected Character character;

    protected bool hasTarget, needMoreTarget;
    protected Pawn target;
    protected List<Pawn> targets = new List<Pawn>();
    protected float timeToGetClosestTarget = 5.0f;
    protected float nextClosestTarget;

    protected PathState pathState;
    protected NavigationRequest navigationRequest;
    protected Vector3[] waypoints = new Vector3[8];
    protected int pathLength, currentPoint;
    [SerializeField]
    protected float pathArrival—orrection = 0.25f;
    [SerializeField]
    protected float maxTargetOffsetToRebuildPath = 5.0f;
    protected float lastDistanceToPoint = 0;

    [SerializeField]
    protected float timeToRebuildPath = 10.0f;
    protected float cooldownToCreateTime = 1.0f;
    protected float nextRebuild, next—ooldown;

    [SerializeField]
    protected float distanceToLostTargetVisibility = 100.0f;
    [SerializeField]
    protected float timeToCheckTargetVisibility = 1.0f;
    protected float nextCheckVisibility;
    protected bool targetIsVisible;

   

    protected void Awake()
    {
        navigationRequest = new NavigationRequest(NavigationRequest);
        gameState = GameInstance.Instance.GameState;
    }

    public override void Possess(Pawn pawn)
    {
        base.Possess(pawn);
        character = pawn as Character;
        //character.Perception.BindOnPerceptionDetection(OnPerceptionDetection);
    }
    public override void Unpossess()
    {
        //character.Perception.UnbindOnPerceptionDetection(OnPerceptionDetection);
        nextRebuild = 0.0f;
        currentPoint = 0;
        pathLength = 0;
        pathState = PathState.none;
        targets.Clear();
        hasTarget = false;
        target = null;
        character = null;
        base.Unpossess();
    }
    protected override void OnDeath(DamageStruct ds, RaycastHit raycastHit)
    {
        gameState.PawnDeath(controlledPawn, ds, raycastHit);
        Unpossess();
    }
    protected override void OnHurt(DamageStruct ds, RaycastHit raycastHit)
    {
        gameState.PawnHurt(controlledPawn, ds, raycastHit);
    }
    protected virtual void Jump(int direction = 1)
    {
        character.Jump(direction);
    }
    protected virtual void Move(Vector3 direction)
    {
        character.LookRotate(direction, 360.0f*Time.deltaTime);
        character.Move(direction,false);
    }
    protected void NavigationRequest(bool hasPath, NavMeshPath path)
    {
        if (hasPath && hasPawn)
        {
            pathLength = path.GetCornersNonAlloc(waypoints);
            currentPoint = 0;
            pathState = PathState.has;
            nextRebuild = Time.time + timeToRebuildPath;
            Vector3 distance = (waypoints[currentPoint] - character.transform.position);
            distance.y = 0;
            lastDistanceToPoint = distance.sqrMagnitude;

        }
        else
        {
            next—ooldown = Time.time + cooldownToCreateTime;
            pathState = PathState.none;
        }
    }
    protected bool CreatePathRequest(Vector3 point, bool forceRebuild = false)
    {
        if (Time.time >= next—ooldown)
        {
            if (pathState.Equals(PathState.none) || (forceRebuild && pathState.Equals(PathState.has)))
            {
                navigationRequest.SetPoints(character.transform.position, point);
                GameInstance.Instance.NavigationQueue.RequestPath(navigationRequest);
                pathState = PathState.request;
            }
            else if (pathState.Equals(PathState.request))
            {
                navigationRequest.SetPoints(character.transform.position, point);
            }
            return !pathState.Equals(PathState.none);
        }
        return false;
    }

    protected void MoveByPath()
    {
        if (pathState.Equals(PathState.has))
        {
            Vector3 distance = (waypoints[currentPoint] - character.transform.position);
            if (distance.sqrMagnitude < pathArrival—orrection* pathArrival—orrection)
            {
                currentPoint++;
                if (currentPoint >= pathLength)
                {
                    pathState = PathState.none;
                    currentPoint = 0;
                    pathLength = 0;
                    return;
                }
                else
                {
                    distance = (waypoints[currentPoint] - character.transform.position);
                    distance.y = 0;
                    lastDistanceToPoint = distance.sqrMagnitude;
                }
            }
            
            {
                float xzm = distance.x * distance.x + distance.z * distance.z;
                float ym = distance.y * distance.y;
                if (ym > xzm * 1.25f || xzm >= lastDistanceToPoint * 1.25f)
                {
                    pathState = PathState.none;
                    currentPoint = 0;
                    pathLength = 0;
                }
                else
                {
                    distance.y = 0;
                    Move(distance.normalized);
                }
                if (lastDistanceToPoint > xzm)
                    lastDistanceToPoint = xzm;
            }
           
        }
    }
    protected bool CheckConditionToRebuildPath()
    {
        if (hasTarget && pathState.Equals(PathState.has))
        {
            float offsetDistance = (target.transform.position - waypoints[pathLength - 1]).sqrMagnitude;
            return (Time.time >= nextRebuild || offsetDistance >= maxTargetOffsetToRebuildPath * maxTargetOffsetToRebuildPath);
        }
        return false;
    }
   
    protected void CheckConditionToChangeTarget()
    {
        if ((!needMoreTarget && !hasTarget) ||
            (hasTarget &&
            (!target.Health.IsAlive || target.Health.Team.Equals(controlledPawn.Health.Team) || Time.time >= nextClosestTarget)))
        {
            ChangeTarget();
            nextClosestTarget = Time.time + timeToGetClosestTarget;
        }
    }
    protected virtual bool CheckTargetVisibility()
    {
        float distance = (target.Center - controlledPawn.Center).sqrMagnitude;
        if (distance <= distanceToLostTargetVisibility * distanceToLostTargetVisibility)
        {
            if (Time.time >= nextCheckVisibility)
            {
                nextCheckVisibility = Time.time + timeToCheckTargetVisibility;
                return targetIsVisible = !Physics.Linecast(character.Look, target.Look, (1 << 6) | (1 << 7));
            }
            else
            {
                return targetIsVisible;
            }
        }
        else
        {
            return targetIsVisible = false;
        }
    }
    protected void RefreshTargets()
    {
        if (hasTarget && !target.Health.IsAlive)
        {
            target = null;
            hasTarget = false;
        }
        for (int i = 0; i < targets.Count; i++)
        {
            if (!targets[i].Health.IsAlive)
            {
                targets.RemoveAt(i);
                i--;
            }
            if (needMoreTarget)
                needMoreTarget = controlledPawn.Health.Team.Equals(targets[i].Health.Team);

        }
    }
    protected void ChangeTarget()
    {

        if (!hasTarget)
        {
            if (!ClosestTarget(ref target))
            {
                hasTarget = false;
                needMoreTarget = true;
                return;
            }
        }
        else
        {
            if (target.Health.Team.Equals(controlledPawn.Health.Team))
            {
                if (!ClosestTarget(ref target))
                {
                    hasTarget = false;
                    needMoreTarget = true;
                    return;
                }
            }
        }
        while (!target.Health.IsAlive)
        {
            targets.Remove(target);
            if (!ClosestTarget(ref target))
            {
                hasTarget = false;
                needMoreTarget = true;
                return;
            }
        }
        hasTarget = true;
        return;
    }
    protected bool ClosestTarget(ref Pawn pawn)
    {
        float minM = float.MaxValue;
        float minTmp;
        bool hasTarget = false;
        for (int i = 0; i < targets.Count; i++)
        {
            if (!controlledPawn.Health.Team.Equals(targets[i].Health.Team) && targets[i].Health.Team != Team.world)
            {
                minTmp = (targets[i].transform.position - controlledPawn.transform.position).sqrMagnitude;
                if (minM > minTmp)
                {
                    hasTarget = true;
                    minM = minTmp;
                    pawn = targets[i];
                }
            }
        }
        return hasTarget;
    }
    protected virtual void OnPerceptionDetection(StimulusStruct stimulus)
    {
        if (stimulus.enter)
        {
            if (!targets.Contains(stimulus.target))
            {
                targets.Add(stimulus.target);
                needMoreTarget = false;
            }
        }
        else
        {
            if (hasTarget && target.Equals(stimulus.target))
            {
                targets.Remove(target);
                hasTarget = false;
                target = null;
            }
            else
            {
                if (targets.Contains(stimulus.target))
                {
                    targets.Remove(stimulus.target);
                }
            }
        }
    }
    protected virtual void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            if (pathState.Equals(PathState.has))
            {
                for (int i = 0; i < pathLength; i++)
                {
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawWireCube(waypoints[i], Vector3.one * 0.5f);
                }
            }
        }
    }
}
