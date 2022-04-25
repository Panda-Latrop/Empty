using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideComponent : MonoBehaviour
{
    protected MovementComponent movement;
    protected CrouchComponent crouch;
    protected bool isSlide;
    [SerializeField]
    protected float speedMultiply = 2.0f;
    [SerializeField]
    protected float slideTime = 1.5f, timeToRecovery = 1.0f, maxMovementDeltaToSlide = 0.0125f;
    protected float nextSlide;
    protected Vector3 direction;
    protected Vector3 lastPos;
    protected float lowerY;
    protected float inSlideTime;
    public bool IsSlide => isSlide;
    public float SpeedMultiply => speedMultiply;
    public void SetMovement(MovementComponent movement)
    {
        this.movement = movement;
    }
    public void SetCrouch(CrouchComponent crouch)
    {
        this.crouch = crouch;
    }
    public void Slide(bool slide, Vector3 direction)
    {
        direction.y = 0;
        direction.Normalize();
        if (slide && direction.sqrMagnitude > 0.0f)
        {
            if (!isSlide && Time.time >= nextSlide)
            {
                isSlide = true;
                this.direction = direction;
                crouch.Crouch(true);
                lastPos = transform.position - Vector3.one * 2.0f;
                inSlideTime = 0.0f;
                lowerY = transform.position.y;
                nextSlide = Time.time + slideTime;
                enabled = true;
            }
        }
        else
        {
            if (isSlide)
            {
                isSlide = false;
                crouch.Crouch(false);
                nextSlide = Time.time + timeToRecovery;
                //Debug.Log("call");
            }
        }
    }
    protected void FixedUpdate()
    {
        if (isSlide)
        {
            Vector3 posDelta = (transform.position - lastPos);
            posDelta.y = 0.0f;
            float delta = 0;
            if (lowerY > transform.position.y)
                lowerY = transform.position.y;
            else
            {
                delta = transform.position.y - lowerY;
            }
            if (movement.Grounded &&
                ((posDelta.sqrMagnitude > maxMovementDeltaToSlide && inSlideTime >= 0.25f) || inSlideTime < 0.25f) &&
                delta <= 1.0f &&
                Time.time < nextSlide)
            {
                movement.Move(direction, false);

            }
            else
            {
                Slide(false, Vector3.zero);
            }
            inSlideTime += Time.deltaTime;
            lastPos = transform.position;
        }
        else
        {
            enabled = false;
        }
    }
}
