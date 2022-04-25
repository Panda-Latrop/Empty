using UnityEngine;

public class DestroyableSingleton<T> : MonoBehaviour where T : DestroyableSingleton<T>
{
    public static T Instance { get; private set; }
    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = (T)this;
        }
        else if (Instance != (T)this)
        {
            Destroy(this);
        }
    }
}