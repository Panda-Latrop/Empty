using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
public class TriggerOnTimelineEnd : MonoBehaviour
{
    public string level;
    public PlayableDirector director;

    void OnEnable()
    {
        director.stopped += OnPlayableDirectorStopped;

    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void OnPlayableDirectorStopped(PlayableDirector aDirector)
    {
        if (director == aDirector)
        {
            GameInstance.Instance.LoadScene(level, 0, 0);
        }
    }
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        GameInstance.Instance.LoadScene(level, 0, 0);
    }
    void OnDisable()
    {
        director.stopped -= OnPlayableDirectorStopped;
    }
}
