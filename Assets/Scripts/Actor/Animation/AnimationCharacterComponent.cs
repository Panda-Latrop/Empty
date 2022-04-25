using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCharacterComponent : AnimationComponent
{
    protected new Rigidbody rigidbody;
    protected MovementComponent movement;
    protected SprintComponent sprint;
    protected CrouchComponent crouch;
    protected SlideComponent slide;
    protected int
        moveHash = Animator.StringToHash("Move"),
        runHash = Animator.StringToHash("Run"),
        changeHash = Animator.StringToHash("Change"),
        equipHash = Animator.StringToHash("Equip"),
        reloadHash = Animator.StringToHash("Reload"),
        reloadStateHash = Animator.StringToHash("ReloadState"),
        shootHash = Animator.StringToHash("Shoot"),
        shootProcessHash = Animator.StringToHash("ShootProcess"),
        changeSpeedHash = Animator.StringToHash("ChangeSpeed"),
        aimXHash = Animator.StringToHash("AimX"),
        aimYHash = Animator.StringToHash("AimY"),
        hurtHash = Animator.StringToHash("Hurt"),
        hurtTypeHash = Animator.StringToHash("HurtType"),
        crouchHash = Animator.StringToHash("Crouch");
    [SerializeField]
    protected int locomotionLayer, weaponLayer, aimLayer;
    [SerializeField]
    protected int hurtAnimationCount = 2;

    public void SetRigidbody(Rigidbody rigidbody)
    {
        this.rigidbody = rigidbody;
    }
    public void SetMovement(MovementComponent movement)
    {
        this.movement = movement;
    }
    public void SetSprint(SprintComponent sprint)
    {
        this.sprint = sprint;
    }
    public void SetCrouch(CrouchComponent crouch)
    {
        this.crouch = crouch;
    }
    public void SetSlide(SlideComponent slide)
    {
        this.slide = slide;
    }

    //protected void Start()
    //{
    //    //moveHash = Animator.StringToHash("Move");
    //    //runHash = Animator.StringToHash("Run");
    //    //changeHash = Animator.StringToHash("Change");
    //    //equipHash = Animator.StringToHash("Equip");
    //    //reloadHash = Animator.StringToHash("Reload");
    //    //reloadStateHash = Animator.StringToHash("ReloadState"); 
    //    //shootHash = Animator.StringToHash("Shoot");
    //    //changeSpeedHash = Animator.StringToHash("ChangeSpeed");
    //}
    public void PlayChange(float speed)
    {
        animator.SetFloat(changeSpeedHash, 1.0f / speed);
        animator.SetTrigger(changeHash);
    }
    public void PlayEquip(float speed)
    {
        animator.SetFloat(changeSpeedHash, 1.0f / speed);
        animator.SetTrigger(equipHash);
    }
    public void PlayReload(bool reload)
    {
        if(reload)
        animator.SetTrigger(reloadHash);
        animator.SetBool(reloadStateHash, reload);
    }
    public void PlayHurt()
    {
        animator.SetTrigger(hurtHash);
        animator.SetInteger(hurtTypeHash, Random.Range(0, hurtAnimationCount));
    }
    public void PlayShoot()
    {       
        animator.Play(shootHash,weaponLayer,0.0f);
    }
    public void SetProcessShoot(int process)
    {
        //Debug.Log(process);
        animator.SetInteger(shootProcessHash, process);
    }
    public void StopCrouch()
    {
        animator.SetBool(crouchHash, false);
    }
    public void SetAim(Vector2 aim)
    {
        animator.SetFloat(aimXHash, aim.x);
        animator.SetFloat(aimYHash, aim.y);
    }
    protected void LateUpdate()
    {
        bool isMove = movement.IsMove && movement.Grounded && !slide.IsSlide;
        animator.SetBool(moveHash, isMove);
        animator.SetBool(runHash, isMove && sprint.IsSprint && !crouch.IsCrouch);
        animator.SetBool(crouchHash, crouch.IsCrouch);
    }

    public void SetWeaponLayerWeight(float weight)
    {
        SetLayerWeight(weaponLayer, weight);
    }
    public float GetWeaponLayerWeight()
    {
       return GetLayerWeight(weaponLayer);
    }
    public void SetAimLayerWeight(float weight)
    {
        SetLayerWeight(aimLayer, weight);
    }
    public float GetAimLayerWeight()
    {
        return GetLayerWeight(aimLayer);
    }
    public override void AnimatorMessage(string message, int value = 0)
    {
       
    }
}
