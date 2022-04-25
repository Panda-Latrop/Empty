using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprintComponent : MonoBehaviour, ISaveableComponent
{
    protected MovementComponent movement;
    protected bool isSprint, canRecovery;
    [SerializeField]
    protected float maxStamina = 100.0f;
    protected float currentStamina = -1.0f;
    [SerializeField]
    protected float speedMultiply = 1.75f;
    [SerializeField]
    protected float staminaRecovery = 50.0f, staminaFatigue = 50.0f;
    [SerializeField]
    protected float timeToRecovery = 1.0f;
    protected float nextRecovery;

    public bool IsSprint => isSprint;
    public float SpeedMultiply => speedMultiply;
    public float MaxStamina => maxStamina;
    public float Stamina => currentStamina;
    public void Start()
    {
        if(currentStamina < 0.0f)
        currentStamina = maxStamina;
    }
    public void SetMovement(MovementComponent movement)
    {
        this.movement = movement;
    }
    public void Sprint(bool sprint)
    {
        if (sprint)
        {
            if (!isSprint && currentStamina > 0.0f && movement.IsMove)
            {
                enabled = true;
                isSprint = true;
            }
        }
        else
        {
            if (isSprint)
            {
                isSprint = false;
                canRecovery = false;
                nextRecovery = Time.time + timeToRecovery;
            }
        }
    }
    protected void Update()
    {
        if (isSprint)
        {
            currentStamina -= staminaFatigue * Time.deltaTime;
            if(currentStamina <= 0.0f)
            {
                currentStamina = 0.0f;
                Sprint(false);
            }
        }
        else
        {
            if (canRecovery)
            {
                currentStamina += staminaRecovery * Time.deltaTime;
                if (currentStamina >= maxStamina)
                {
                    currentStamina = maxStamina;
                    enabled = false;
                }
            }
            else
            {
                if (Time.time >= nextRecovery)
                    canRecovery = true;
            }
        }
    }

    public JSONObject Save(JSONObject jsonObject)
    {
        throw new System.NotImplementedException();
    }

    public JSONObject Load(JSONObject jsonObject)
    {
        throw new System.NotImplementedException();
    }
}
