using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Character : Pawn
{
    [SerializeField]
    protected CapsuleCollider capsule;
    [SerializeField]
    protected new Rigidbody rigidbody;
    [SerializeField]
    protected SkeletalMeshComponent skeletal;
    [SerializeField]
    protected DeathComponent death;
    [SerializeField]
    protected MovementComponent movement;
    protected Vector3 lookRotation;
    [SerializeField]
    protected SprintComponent sprint;
    [SerializeField]
    protected CrouchComponent crouch;
    [SerializeField]
    protected SlideComponent slide;
    [SerializeField]
    protected WeaponHolderComponent weaponHolder;
    [SerializeField]
    protected new AnimationCharacterComponent animation;
    [SerializeField]
    protected AudioSource voice;
    public CapsuleCollider Capsule { get => capsule; }
    public Rigidbody Rigidbody { get => rigidbody; }
    public MovementComponent Movement { get => movement; }
    public SprintComponent Sprinting { get => sprint; }
    public CrouchComponent Crouching { get => crouch; }
    public WeaponHolderComponent WeaponHolder { get => weaponHolder; }
    public override Vector3 Center => capsule.bounds.center;
    public override Bounds Bounds => capsule.bounds;
    public override Vector3 LookDirection => lookTransform.forward;
    public void  PlayVoice(AudioClip clip)
    {
        voice.clip = clip;
        voice.Play();
    }
    public override void LookRotate(Vector3 angle)
    {      
        lookRotation = Utility.ClampRotation(lookRotation + angle, -90.0f, 90.0f);
        rigidbody.MoveRotation(Quaternion.AngleAxis(lookRotation.y, Vector3.up));
    }
    public override void LookRotate(Vector3 direction,float displacement)
    {
        lookRotation = Utility.CharacterRotateToDirection(lookRotation, direction,displacement);
        lookRotation.y = Utility.ClampRotation(lookRotation.y);
        rigidbody.MoveRotation(Quaternion.AngleAxis(lookRotation.y, Vector3.up));
    }
    public void LookAim(Vector3 target, float displacement)
    {

        //Vector3 distance = target - lookTransform.position;
        //Vector3 direction = distance;
        //direction.Normalize();
        //lookRotation = Utility.CharacterRotateToDirection(lookRotation, direction, displacement);
        //lookRotation.y = Utility.ClampRotation(lookRotation.y);
        //rigidbody.MoveRotation(Quaternion.AngleAxis(lookRotation.y, Vector3.up));
        //Vector2 aim = Vector3.zero;
        //aim.y = distance.y;
        //animation.SetAim(aim);
        //return direction;

        Vector3 distance = target - lookTransform.position;
        Vector3 direction = distance;
        direction.Normalize();
        LookRotate(direction, displacement);
        direction.x = direction.z = 0;
        animation.SetAim(direction);
    }
    public void SetWeaponLayerWeight(float weight)
    {
        animation.SetWeaponLayerWeight(weight);
    }
    public float GetWeaponLayerWeight()
    {
        return animation.GetWeaponLayerWeight();
    }
    public void SetAimLayerWeight(float weight)
    {
        animation.SetAimLayerWeight(weight);
    }
    public float GetAimLayerWeight()
    {
        return animation.GetAimLayerWeight();
    }
    
    protected override void Start()
    {
        base.Start();
        lookRotation = transform.rotation.eulerAngles;
        lookRotation.y = Utility.ClampRotation(lookRotation.y);
        rigidbody.MoveRotation(Quaternion.AngleAxis(lookRotation.y, Vector3.up));
    }
    protected override void Setup()
    {
        base.Setup();
        weaponHolder.BindOnChange(OnChange);
        weaponHolder.BindOnEquip(OnEquip);
        weaponHolder.BindOnReload(OnReload);
        death.SetOwner(this);
        death.SetSkeletalMesh(skeletal);
        movement.SetRigidbody(rigidbody);
        movement.SetCollider(capsule);
        movement.BindOnJump(OnJump);
        movement.BindOnGround(OnGround);
        sprint.SetMovement(movement);
        crouch.SetCapsule(capsule);
        crouch.SetMovement(movement);
        slide.SetMovement(movement);
        slide.SetCrouch(crouch);
        weaponHolder.SetOwner(this);
        animation.SetRigidbody(rigidbody);
        animation.SetMovement(movement);
        animation.SetSprint(sprint);
        animation.SetCrouch(crouch);
        animation.SetSlide(slide);
    }
    
    protected override void OnDeath(DamageStruct ds, RaycastHit raycastHit)
    {
        capsule.enabled = false;
        rigidbody.velocity = Vector3.zero;
        rigidbody.detectCollisions = false;
        movement.enabled = false;
        death.Death(ds,raycastHit);
        voice.Stop();
        base.OnDeath(ds, raycastHit);

    }
    protected override void OnHurt(DamageStruct ds, RaycastHit raycastHit)
    {
        animation.PlayHurt();
    }
    protected override void OnResurrect()
    {
        capsule.enabled = true;
        rigidbody.detectCollisions = true;
        movement.enabled = true;
        base.OnResurrect();
    }
    public override void OnPop()
    {
        base.OnPop();
        lookRotation = transform.rotation.eulerAngles;
        lookRotation.y = Utility.ClampRotation(lookRotation.y);
        rigidbody.MoveRotation(Quaternion.AngleAxis(lookRotation.y, Vector3.up));
        health.Resurrect();
        weaponHolder.Restore();
        if (!hasController && hasAutoController && health.IsAlive)
        {
            (GameInstance.Instance.PoolManager.Pop(autoController) as ControllerBase).Possess(this);
        }

    }
    public override void SetRotation(Quaternion rotation)
    {
        base.SetRotation(rotation);
        lookRotation = rotation.eulerAngles;
        lookRotation.y = Utility.ClampRotation(lookRotation.y);
        rigidbody.MoveRotation(Quaternion.AngleAxis(lookRotation.y, Vector3.up));
    }
    public override void OnPush()
    {
        base.OnPush();
        transform.rotation = Quaternion.identity;
        lookRotation = Vector3.zero;
    }
    protected virtual void OnJump(){}
    protected virtual void OnGround(){ if (!slide.IsSlide && !crouch.IsCrouch && !sprint.IsSprint) movement.SpeedMultiply = 1.0f; }
    protected virtual void OnChange(WeaponActor weapon)
    {
        animation.PlayChange(weapon.ChangedTime);
    }
    protected virtual void OnEquip(WeaponActor weapon)
    {
        animation.ChanegeController(weaponHolder.IsFirstPerson ? weapon.GetFirstPersonAnimatorController() : weapon.GetThirdPersonAnimatorController());
        animation.PlayEquip(weapon.ChangedTime);
    }
    public virtual void Move(Vector3 direction,bool moveForward = true)
    {
        if (!slide.IsSlide)
            movement.Move(direction, moveForward);
    }
    public virtual void Jump(float direction = 1.0f)
    {
        if (!slide.IsSlide)
            movement.Jump(direction);
    }
    public virtual void Sprint(bool sprint)
    {
        if (!slide.IsSlide && movement.Grounded)
        {
            this.sprint.Sprint(sprint);
        }
 
    }
    public virtual void Crouch(bool crouch)
    {
        if (!slide.IsSlide)
            this.crouch.Crouch(crouch);          
    }
    public virtual void Slide(bool slide)
    {
        if (!this.slide.IsSlide && movement.Grounded)
            this.slide.Slide(slide,movement.Direction);
    }
    public virtual void SetFire(bool fire)
    {
            weaponHolder.SetFire(fire && !sprint.IsSprint);
    }
    public virtual ShootState Shoot(bool fire,Vector3 position, Vector3 direction)
    {
        SetFire(fire);
        return Shoot(position, direction);
    }
    public virtual ShootState Shoot(Vector3 position, Vector3 direction)
    {
        ShootState state = weaponHolder.Shoot(position, direction);
        switch (state)
        {
            case ShootState.initiated:
                animation.PlayShoot();
                animation.SetProcessShoot(0);
                break;
            case ShootState.process:
                animation.SetProcessShoot(1);
                break;
            case ShootState.ended:
                break;
            default:
                break;
        }
        return state;
    }
    public bool HasWeapon => weaponHolder.HasWeapon;
    public bool WeaponAmmoIsEmpty()
    {
        return weaponHolder.GetWeapon().Ammo.IsEmpty;
        
    }
    public bool WeaponAmmoIsFull()
    {
        return weaponHolder.GetWeapon().Ammo.IsFull;

    }
    public void Reload(bool reload = true)
    {
        if (!sprint.IsSprint)
        {
            if (weaponHolder.Reload(reload))
                animation.PlayReload(reload);
        }

    }
    public void OnReload(ReloadState state)
    {
        animation.PlayReload(true);

    }
    protected virtual void Update()
    {
        if (slide.IsSlide)
        {
            if (movement.Grounded)
                movement.SpeedMultiply = slide.SpeedMultiply;
            sprint.Sprint(false);
        }
        else if (crouch.IsCrouch)
        {
            if (movement.Grounded)
                movement.SpeedMultiply = crouch.SpeedMultiply;
            sprint.Sprint(false);
        }
        else if (sprint.IsSprint)
        {
            if (movement.Grounded)
            {
                SetFire(false);
                movement.SpeedMultiply = sprint.SpeedMultiply;
                weaponHolder.Stop();
            }          
            else
                sprint.Sprint(false);
            if (!movement.IsMove)
                sprint.Sprint(false);
        }
        else if (movement.Grounded)
        {
            sprint.Sprint(false);
            movement.SpeedMultiply = 1.0f;
        }
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        weaponHolder.UnbindOnChange(OnChange);
        weaponHolder.UnbindOnEquip(OnEquip);
        weaponHolder.UnbindOnReload(OnReload);
    }
    public override JSONObject Save( JSONObject jsonObject)
    {
        //base.Save( jsonObject);
        //SaveSystem.ColliderSave(jsonObject, "physics", capsule);
        // SaveSystem.RigidbodySave(jsonObject, rigidbody);
        // jsonObject.Add("movement", movement.Save(new JSONObject()));

        //JSONArray lookRotationJArray = new JSONArray();
        //lookRotationJArray.Add(new JSONNumber(lookRotation.x));
        //lookRotationJArray.Add(new JSONNumber(lookRotation.y));
        //lookRotationJArray.Add(new JSONNumber(lookRotation.z));
        //jsonObject.Add("lookRotation", lookRotationJArray);


        //jsonObject.Add("orientation", orientation.Save(new JSONObject()));



        jsonObject.Add("name", new JSONString(gameObject.name));
        jsonObject.Add("prefab", new JSONString(path));
        jsonObject.Add("enabled", new JSONBool(enabled));
        jsonObject.Add("saveTag", new JSONString(saveTag));
        jsonObject.Add("health", health.Save(new JSONObject()));
        jsonObject.Add("weaponHolder", weaponHolder.Save(new JSONObject()));
        return jsonObject;
    }
    public override JSONObject Load( JSONObject jsonObject)
    {
        //base.Load( jsonObject);
        //SaveSystem.ColliderLoad(jsonObject, "physics", capsule);
        //SaveSystem.RigidbodyLoad(jsonObject, rigidbody);
        //movement.Load(jsonObject["movement"].AsObject);

        //JSONArray lookRotationJArray = jsonObject["lookRotation"].AsArray;
        //lookRotation = new Vector3(lookRotationJArray[0].AsFloat, lookRotationJArray[1].AsFloat, lookRotationJArray[2].AsFloat);
        //rigidbody.MoveRotation(Quaternion.AngleAxis(lookRotation.y, Vector3.up));


        //orientation.Load(jsonObject["orientation"].AsObject);

        gameObject.name = jsonObject["name"];
        enabled = jsonObject["enabled"].AsBool;
        saveTag = jsonObject["saveTag"];

        health.Load(jsonObject["health"].AsObject);
        weaponHolder.Load(jsonObject["weaponHolder"].AsObject);
        return jsonObject;
    }
}