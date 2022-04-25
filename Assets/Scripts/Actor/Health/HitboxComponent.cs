using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxComponent : MonoBehaviour, IHealth
{
    [SerializeField]
    protected HealthComponent health;
    [SerializeField]
    protected float damageModifier = 1.0f;
    public bool IsAlive => health.IsAlive;

    public Team Team { get => health.Team; set => health.Team = value; }

    public void Heal(float health)
    {
        this.health.Heal(health);
    }
    public HurtResult Hurt(DamageStruct ds, RaycastHit raycastHit)
    {
        ds.damage = ds.damage * damageModifier;
        ds.bone = name;
        return health.Hurt(ds, raycastHit);
    }
}
