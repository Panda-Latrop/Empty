using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T Instance { get; private set; }
    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = (T)this;
            DontDestroyOnLoad(Instance.gameObject);
        }
        else
        {
            if (Instance != (T)this)
            {
                //Destroy(gameObject);
                Destroy(this);
            }
        }
    }
}
