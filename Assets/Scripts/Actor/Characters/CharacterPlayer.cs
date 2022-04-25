using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPlayer : Character
{
    [SerializeField]
    protected new CameraPlayerFPSActor camera;
    [SerializeField]
    protected PlayerMovementSoundComponent movementSound;
    protected override void Setup()
    {
        base.Setup();
        camera.Animation.SetMovement(movement);
        camera.Animation.SetSprint(sprint);
        camera.Animation.SetCrouch(crouch);
        camera.Animation.SetSlide(slide);

        movementSound.SetCollider(capsule);
    }
    public override void LookRotate(Vector3 angle)
    {
        float x = angle.x;
        angle.x = 0.0f;
        lookRotation = Utility.ClampRotation(lookRotation + angle, -90.0f, 90.0f);
        rigidbody.MoveRotation(Quaternion.AngleAxis(lookRotation.y, Vector3.up));
        camera.LookRotate(Vector3.right * x);
    }
    public override Vector3 LookDirection => camera.transform.forward;
    public override ShootState Shoot(Vector3 position, Vector3 direction)
    {
        ShootState state = base.Shoot(camera.transform.position, camera.transform.forward);
        if (state.Equals(ShootState.initiated))
            camera.PlayRecoil(weaponHolder.GetWeapon().RecoilCurve);
        return state;
    }
    protected override void OnDeath(DamageStruct ds, RaycastHit raycastHit)
    {
        rigidbody.useGravity = true;
        rigidbody.velocity = Vector3.zero;
        movement.enabled = false;
        enabled = false;
        crouch.enabled = false;
        weaponHolder.Change(-1);
        camera.PlayDeath();
    }
    protected override void OnResurrect()
    {
        rigidbody.useGravity = false;
        movement.enabled = true;
        crouch.enabled = true;
        enabled = true;
        camera.StopDeath();
    }
    protected override void OnHurt(DamageStruct ds, RaycastHit raycastHit)
    {
        
    }
    protected override void OnJump()
    {
        camera.PlayJump();
        movementSound.PlayJump();
    }
    protected override void OnGround()
    {
        base.OnGround();
        camera.PlayGround();
        movementSound.PlayGround();
    }
    public override void Crouch(bool crouch)
    {
        base.Crouch(crouch);
        //camera.Animation.SetCrouch();
    }
    protected override void Update()
    {
        base.Update();

        if (movement.IsMove && movement.Grounded)
        {
            if (slide.IsSlide)
            {
                movementSound.PlaySlide();
            }
            else
            {
                movementSound.StopSlide();
                if (sprint.IsSprint)
                    movementSound.PlayFootstep(2.0f);
                else
                    movementSound.PlayFootstep(); 
            }
        }
        else
        {
            movementSound.StopSlide();
            movementSound.StopFootstep();
        }
    }
}
