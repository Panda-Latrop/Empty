using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "Music Holder", menuName = "System/Music/Holder", order = 2)]
public class MusicHolder : ScriptableObject
{
    [SerializeField]
    protected AudioMixerGroup mixerGroup;
    [SerializeField]
    protected AudioClip[] music;

    public AudioMixerGroup GetMixerGroup()
    {
        return mixerGroup;
    }

    public AudioClip Get(int music)
    {
        return this.music[music];
    }
    public int GetCount()
    {
        return music.Length;
    }
}