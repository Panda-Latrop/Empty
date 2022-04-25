using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Audio Cue", menuName = "Audio/Cue", order = 2)]
public class AudioCueScriptableObject : ScriptableObject
{
    [SerializeField]
    public AudioClip[] clips;

    public AudioClip Get()
    {
        return clips[Random.Range(0, clips.Length)];
    }

    public static implicit operator AudioClip(AudioCueScriptableObject cue)
    {
        return cue.Get();
    }

}
