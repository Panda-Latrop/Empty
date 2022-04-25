using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIShooterController : AIController
{
    [SerializeField]
    protected float rotateSpeed = 180.0f;
    [SerializeField]
    protected float distanceToShoot;
    [SerializeField]
    protected float distanceToClose, distanceToFar;
    protected bool closeToTarget;

    protected bool lostTarget = true;

    [SerializeField]
    protected float vectorDotToShoot = 0.7f;
    protected bool weaponAmmoIsNotEmpty = true;
    protected float aimWeight = 0.0f;

    [SerializeField]
    protected float timeToPrepare = 0.5f, timeToReload = 1.0f;
    protected float nextReload, nextPrepare;
    [SerializeField]
    protected float timeToCrouch = 1.0f;
    protected float nextCrouch;
    [SerializeField]
    protected float timeToStrafe = 3.0f;
    protected float nextStrafe;
    [SerializeField]
    protected float strafeDistnace = 1.5f;
    protected bool inStrafe;
    protected Vector3 strafePoint;

    [SerializeField]
    protected bool useBrust = false;
    [SerializeField]
    protected int maxBrust = 4;
    protected int brustCount;
    [SerializeField]
    protected float timeToBrust = 2.0f;
    protected float nextBrust;
    protected float timeBetweenShoot = 1.0f;
    protected float nextBetweenShoot;
    [SerializeField]
    protected float timeToVoice = 10.0f;
    protected float nextVoice;
    [SerializeField]
    protected AudioCueScriptableObject voiceBattle, voicePain;

    public override void Possess(Pawn pawn)
    {
        base.Possess(pawn);
        target = GameInstance.Instance.PlayerController.ControlledPawn;
        hasTarget = true;
        character.SetWeaponLayerWeight(1);
        nextCrouch = Time.time + timeToCrouch * Random.Range(0.75f, 1.25f);
        nextStrafe = Time.time + timeToStrafe * Random.Range(0.75f, 1.25f);
        nextBetweenShoot = Time.time + timeBetweenShoot * Random.Range(0.0f, 1.25f);
        nextVoice = Time.time + timeToVoice * Random.Range(0.0f, 1.25f);
        nextReload = nextPrepare = 0;
    }
    protected override void OnDeath(DamageStruct ds, RaycastHit raycastHit)
    {
        base.OnDeath(ds, raycastHit);
        closeToTarget = false;
        lostTarget = true;
        weaponAmmoIsNotEmpty = true;
        nextCrouch = Time.time + timeToCrouch * Random.Range(0.75f, 1.25f);
        hasTarget = false;
        nextStrafe = Time.time + timeToStrafe * Random.Range(0.75f, 1.25f);
        brustCount = 0;
        //Debug.Break();
    }

    protected override void OnHurt(DamageStruct ds, RaycastHit raycastHit)
    {
        nextPrepare = Time.time + timeToPrepare * Random.Range(0.75f, 1.25f);
        nextCrouch = nextCrouch - timeToCrouch * Random.Range(0.1f, 0.3f);
        character.PlayVoice(voicePain);
        nextVoice = Time.time + timeToVoice * Random.Range(0.25f, 1.25f);
        base.OnHurt(ds, raycastHit);
    }


    protected virtual void Update()
    {
        if (hasTarget)
        {
            Vector3 distance = (target.Look - character.Look);
            Vector3 direction = distance;
            direction.Normalize();
            if (CheckTargetVisibility())
            {
                float dirDot = Vector3.Dot(character.LookDirection, direction);
                if (aimWeight < 1.0f)
                {
                    aimWeight = Mathf.Lerp(aimWeight, 1.0f, 0.25f);
                    character.SetAimLayerWeight(aimWeight);
                }
                character.LookAim(target.Look, rotateSpeed * Time.deltaTime);

                if (Time.time >= nextReload && Time.time >= nextPrepare)
                {
                    if (weaponAmmoIsNotEmpty)
                    {
                        if (dirDot > vectorDotToShoot)
                        {
                            if (useBrust)
                            {
                                if(Time.time >= nextBrust && character.Shoot(true, character.Look, direction).Equals(ShootState.initiated))
                                {
                                    brustCount++;
                                    if(brustCount >= maxBrust)
                                    {
                                        brustCount = 0;
                                        nextBrust = Time.time + timeToBrust * Random.Range(1.0f, 1.25f); ;
                                    }
                                }
                            }
                            else
                            {
                                if (Time.time >= nextBetweenShoot && character.Shoot(true, character.Look, direction).Equals(ShootState.initiated))
                                {
                                    nextBetweenShoot = Time.time + timeBetweenShoot * Random.Range(0.0f, 2.0f);
                                }
                            }
                        }
                            
                        if (!(weaponAmmoIsNotEmpty = !character.WeaponAmmoIsEmpty()))
                            nextReload = Time.time + timeToReload * Random.Range(1.0f, 1.25f);
                    }
                    else
                    {
                        character.SetFire(false);
                        character.Reload(true);
                        if (weaponAmmoIsNotEmpty = character.WeaponAmmoIsFull())
                            nextReload = Time.time + timeToReload * Random.Range(1.0f, 1.25f);
                    }
                }

                if (CloseToTarget())
                {
                    if (inStrafe)
                    {
                        StrafeMovement();
                    }
                    else
                    {
                        if (pathState.Equals(PathState.has))
                            pathState = PathState.none;
                        if (Time.time >= nextCrouch)
                        {
                            if (Random.Range(0, 2) == 1)
                                character.Crouch(!character.Crouching.IsCrouch);
                            nextCrouch = Time.time + timeToCrouch * Random.Range(0.75f, 1.25f);
                            nextStrafe = Time.time + timeToStrafe * Random.Range(0.75f, 1.25f);
                        }
                        if (Time.time >= nextStrafe && !character.Crouching.IsCrouch)
                        {
                            inStrafe = true;
                            strafePoint =  Random.insideUnitCircle * strafeDistnace;
                            strafePoint.z = strafePoint.y;
                            strafePoint.y = 0;
                            strafePoint += controlledPawn.transform.position;
                        }
                    }

                }
                else
                {
                    character.Crouch(false);
                    Movement();
                }

            }
            else
            {
                if (aimWeight > 0.0f)
                {
                    aimWeight = Mathf.Lerp(aimWeight, 0.0f, 0.25f);
                    character.SetAimLayerWeight(aimWeight);
                }
                character.SetFire(false);
                character.Crouch(false);
                if (!lostTarget)
                    SearchMovement();
            }

            if(Time.time >= nextVoice)
            {
                character.PlayVoice(voiceBattle);
                nextVoice = Time.time + timeToVoice * Random.Range(0.25f, 1.25f);
            }

        }
    }
    protected void LateUpdate()
    {
        hasTarget = target.Health.IsAlive;
    }
    protected bool CloseToTarget()
    {
        float distance = (target.Look - character.Look).sqrMagnitude;      
        if (distance <= distanceToClose * distanceToClose && !closeToTarget)
        {
            nextStrafe = Time.time + timeToStrafe * Random.Range(0.75f, 1.25f);
            return closeToTarget = true;
        }        
        else
        {
            if (distance > distanceToFar * distanceToFar && closeToTarget)
            {
                inStrafe = false;
                return closeToTarget = false;
            }
                
        }
        return closeToTarget;
    }

    protected override bool CheckTargetVisibility()
    {
        bool vis = targetIsVisible;
        base.CheckTargetVisibility();
        if (vis != targetIsVisible)
        {
            if (targetIsVisible)
            {
                lostTarget = false;
                nextPrepare = Time.time + timeToPrepare * Random.Range(0.75f, 1.25f);
                pathState = PathState.none;
                currentPoint = 0;
                pathLength = 0;
            }
            else
            {
                inStrafe = false;
            }
        }
        return targetIsVisible;
    }


    protected void StrafeMovement()
    {
        if (pathState.Equals(PathState.has))
        {
            MoveByPath();
            if (pathState.Equals(PathState.none))
            {
                inStrafe = false;
                nextStrafe = Time.time + timeToStrafe *Random.Range(0.75f, 1.25f);
            }
        }
        else if (pathState.Equals(PathState.none))
        {
            //Debug.Log(pathState);
            if (!CreatePathRequest(strafePoint))
            {
                inStrafe = false;
                nextStrafe = Time.time + timeToStrafe * Random.Range(0.75f, 1.25f);
            }
        }
    }
        protected void SearchMovement()
    {
        if (pathState.Equals(PathState.has))
        {
            MoveByPath();
            if (pathState.Equals(PathState.none))
            {
                lostTarget = true;
            }               
        }
        else if (pathState.Equals(PathState.none))
        {
            CreatePathRequest(target.transform.position);
        }    
    }
    protected override void Move(Vector3 direction)
    {
        if(!targetIsVisible)
        character.LookRotate(direction, rotateSpeed * Time.deltaTime);
        character.Move(direction, false);
    }
    protected void Movement()
    {
        Vector3 distance = (target.Center - character.Center);
        Vector3 direction = distance;
        direction.y = 0;
        direction.Normalize();     
        float xzm = distance.x * distance.x + distance.z * distance.z;
        float ym = distance.y * distance.y;
        //character.LookRotate(direction, 360.0f * Time.deltaTime);
        if (xzm < 10.0f && ym < 4.0f)
        {
            //character.Move(distance.normalized, false);
            Move(direction);
        }
        else
        {
            if (pathState.Equals(PathState.has))
            {
                MoveByPath();
                if (CheckConditionToRebuildPath())
                    CreatePathRequest(target.transform.position, true);
            }
            else if (pathState.Equals(PathState.none))
            {
                CreatePathRequest(target.transform.position);
            }
        }
    }
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(controlledPawn.Center, distanceToFar);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(controlledPawn.Center, distanceToClose);
    }
}
