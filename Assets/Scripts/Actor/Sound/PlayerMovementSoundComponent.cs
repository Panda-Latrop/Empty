using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementSoundComponent: MonoBehaviour
{
    protected bool playingSlide;
    protected new Collider collider;
    protected float height,radius;
    [SerializeField]
    protected AudioSource source;
    [SerializeField]
    protected AudioFootstepMapScriptableObject footsteps;
    [SerializeField]
    protected AudioClip slideSound,slideUpSound,sprintSound;
    [SerializeField]
    protected float timeToStep = 0.5f;
    protected float nextStep;

    [SerializeField]
    protected float timeToPlayGround = 1.0f;
    protected bool hasLastGroundTime;
    protected float lastGroundTime;

    protected PhysicMaterial current;
    protected AudioCueScriptableObject cue;

    public void SetCollider(Collider collider)
    {
        this.collider = collider;
        height = collider.bounds.extents.y;
        radius = collider.bounds.extents.x*0.8f;
    }
    protected void PlaySound(AudioClip clip)
    {
        source.Stop();
        source.time = 0.0f;
        source.clip = clip;
        source.Play();
    }
    protected void CheckSurface()
    {
        Vector3 center = collider.bounds.center;
        RaycastHit hit;
        if (Physics.SphereCast(center, radius, Vector3.down, out hit, height * 2, (1 << 6) | (1 << 7), QueryTriggerInteraction.Ignore))
        {
            if (current != hit.collider.sharedMaterial)
            {
                current = hit.collider.sharedMaterial;
                cue = footsteps.GetSound(current);
            }
        }
    }
    public void PlayJump()
    {
        CheckSurface();
        PlaySound(cue);
        lastGroundTime = Time.time;
        nextStep = 0.0f;
    }
   
    public void PlayGround()
    {
        if (Time.time - lastGroundTime >= timeToPlayGround)
        {
            CheckSurface();          
            PlaySound(cue);
            nextStep = 0.0f;        
        }   
    }
    public void PlayFootstep(float speed = 1.0f)
    {
        nextStep += speed * Time.deltaTime;
        if (nextStep >= timeToStep)
        {
            CheckSurface();
            PlaySound(cue);
            nextStep = nextStep - timeToStep;
        }
    }
    public void StopFootstep()
    {
        nextStep = 0.0f;
        source.loop = false;
    }
    public void PlaySlide()
    {
        if (!playingSlide)
        {
            nextStep = 0.0f;
            source.loop = true;
            playingSlide = true;
            PlaySound(slideSound);
            
        }
    }
    public void StopSlide()
    {
        if (playingSlide)
        {
            nextStep = 0.0f;
            source.loop = false;
            playingSlide = false;
            PlaySound(slideUpSound);
        }
    }


}

