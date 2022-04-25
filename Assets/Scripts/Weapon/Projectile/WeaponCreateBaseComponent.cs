using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponCreateBaseComponent : MonoBehaviour
{
    public abstract HurtResult CreateProjectile(Vector3 position, Vector3 direction, float distance, float speed, DamageStruct ds);

}
