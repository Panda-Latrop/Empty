using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSoundComponent : MonoBehaviour
{
    [SerializeField]
    protected AudioSource source;
    [SerializeField]
    protected AudioClip shoot;

    public void Player()
    {
        source.clip = shoot;
        source.time = 0.0f;
        source.Play();
    }
}
