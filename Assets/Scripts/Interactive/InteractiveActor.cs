using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractive
{
    void Intercat(Character by);
}

public abstract class InteractiveActor : Actor, IInteractive
{
    public abstract void Intercat(Character by);
}
