using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicSystem : MonoBehaviour
{
    [SerializeField]
    protected MusicHolder musicHolder;
    [SerializeField]
    protected AudioSource audioSource;
    protected bool canChange = true, softChange;
    protected int changeState;
    protected int musicId;
    protected string path = string.Empty;
    protected AudioClip forChange;
    protected float fadeInSpeed = 2.0f, fadeOutSpeed = 1.25f;
    protected float time,softChangeTime = 3.0f,nextSoftChange;


    protected int currentMusic;
    public void SetMusicHolder(MusicHolder musicHolder)
    {
        if (this.musicHolder == null || !this.musicHolder.name.Equals(musicHolder.name) || !audioSource.isPlaying)
        {
            this.musicHolder = musicHolder;
            Change();
        }
    }
    public void SetAudioSource(AudioSource audioSource,AudioMixerGroup mixerGroup)
    {
        this.audioSource = audioSource;
        this.audioSource.spatialBlend = 0.0f;
        this.audioSource.outputAudioMixerGroup = mixerGroup;
    }
    public void Stop()
    {
        audioSource.Stop();
        changeState = 0;
        canChange = true;
    }
    public void Pause()
    {
        audioSource.Pause();
    }
    public void Resume()
    {
        audioSource.UnPause();
    }
    public void Change(string path)
    {
        this.path = path;
        Change(Resources.Load(path) as AudioClip,false);
    }

    [ContextMenu("Change")]
    public void Change()
    {
        path = string.Empty;
        var rand = Random.Range(0, musicHolder.GetCount());
        if (rand == musicId)
        {
            musicId++;
            if (musicId >= musicHolder.GetCount())
                musicId = 0;
        }
        else
        {
            musicId = rand;
        }
        Change(musicHolder.Get(musicId));
    }
    public void Change(AudioClip music, bool canChange = true)
    {
        if (softChange)
        {
            changeState = 3;
            nextSoftChange = Time.time + softChangeTime;
            softChange = false;
        }          
        else
            changeState = 1;
        forChange = music;
        this.canChange = canChange;
    }
    protected void LateUpdate()
    {
        switch (changeState)
        {
            case 1:
                audioSource.volume -= fadeOutSpeed * Time.deltaTime;
                if (audioSource.volume <= 0 || !audioSource.isPlaying)
                {
                    audioSource.volume = 0;
                    audioSource.Stop();
                    audioSource.time = 0;
                    audioSource.clip = forChange;
                    audioSource.Play();
                    changeState = 2;
                } 
                break;
            case 2:
                audioSource.volume += fadeInSpeed * Time.deltaTime;
                if (audioSource.volume >= 1)
                {
                    audioSource.volume = 1;
                    changeState = 0;
                }
                break;
            case 3:
                if (Time.time >= nextSoftChange)
                {
                    changeState = 1;
                }
                break;
            default:
                if (!audioSource.isPlaying && audioSource.time == 0)
                {
                    if (canChange)
                        Change();
                    else
                        audioSource.Play();
                }
                if (audioSource.volume < 1)
                {
                    audioSource.volume += fadeInSpeed * Time.deltaTime;
                }
                break; 
        }
        time = audioSource.time;
    }
}
