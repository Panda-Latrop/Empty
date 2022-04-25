using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleDoorActor : Actor
{
    [SerializeField]
    protected bool isOpen;
    [SerializeField]
    protected Animator animator;
    [SerializeField]
    protected AudioSource source;
    protected int toOpenHash = Animator.StringToHash("ToOpen"),
                  toCloseHash = Animator.StringToHash("ToClose");
    [SerializeField]
    protected bool useTrigger = false;
    [SerializeField]
    protected TriggerActor trigger;

    public void Awake()
    {
        if(useTrigger)
        trigger.BindOnExecute(OnTrigger);
    }

    protected void Start()
    {
        if (isOpen)
            Open(); 
        else
            Close();
    }

    public virtual void Open()
    {
        animator.Play(toOpenHash);
        source.Play();
        isOpen = true;
    }
    public virtual void Close()
    {
        animator.Play(toCloseHash);
        source.Play();
        isOpen = false;
    }

    protected void OnTrigger()
    {
        if (isOpen)
            Close(); 
        else
            Open();
        //isOpen = !isOpen;
    }
}
