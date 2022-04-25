using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMultiShooter : WeaponShooter
{
    [SerializeField]
    [Range(2,20)]
    protected int projectileInShoot = 8;
    protected override HurtResult CreateProjectile(Vector3 position, Vector3 direction)
    {
        HurtResult result = HurtResult.none;
        for (int i = 0; i < projectileInShoot; i++)
        {
            result = base.CreateProjectile(position, direction);
        }
        return result;
    }
}
