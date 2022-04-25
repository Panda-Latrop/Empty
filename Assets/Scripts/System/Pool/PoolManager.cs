using System.Collections.Generic;
using UnityEngine;


[System.Flags]
public enum PoolPopInfo
{
    failure = 0,
    done = 1,
    force = 2
}
public class PoolManager : MonoBehaviour
{
    
    public class Pool
    {
        protected readonly Queue<IPoolObject> objects;
        protected string prefab;
        protected Transform root;
        public string Prefab { get => prefab; }
        public Transform Root { get => root; }

        public Pool(string prefab, Transform root)
        {
            this.prefab = prefab;
            this.root = root;
            objects = new Queue<IPoolObject>();
        }
        public void Push(IPoolObject poolObject)
        {
            poolObject.OnPush();
            objects.Enqueue(poolObject);
        }
        public IPoolObject Pop()
        {
            IPoolObject result = default;
            if (objects.Count > 0)
            {
                result = objects.Dequeue();
                result.OnPop();
            }
            return result;
        }
    }
    public Dictionary<string, Pool> pools = new Dictionary<string, Pool>();
    public bool AddPool(IPoolObject poolObject)
    {
        if (poolObject != null && !pools.ContainsKey(poolObject.PoolTag))
        {
            string prefab = poolObject.Path;
            Transform root = CreateRoot(poolObject.PoolTag).transform;
            Pool pool = new Pool(prefab, root);
            pools.Add(poolObject.PoolTag, pool);
            return true;
        }
        Debug.LogWarning("Can't Add To Pool");
        return false;
    }
    public bool AddPool(GameObject gameObject)
    {
        return AddPool(gameObject.GetComponent<IPoolObject>());
    }
    public void Push(IPoolObject poolObject, bool addPoolOnFailure = true)
    {
        if (pools.ContainsKey(poolObject.PoolTag))
        {
           // poolObject.SetPosition(pools[poolObject.PoolTag].Root.position);
           // poolObject.SetRotation(Quaternion.identity);
            poolObject.SetParent(pools[poolObject.PoolTag].Root);
            pools[poolObject.PoolTag].Push(poolObject);
        }
        else
        {
            if (addPoolOnFailure)
            {

                if (AddPool(poolObject))
                {
                    Push(poolObject, addPoolOnFailure);
                }
            }
        }     
    }
    public IPoolObject Pop(IPoolObject poolObject, out PoolPopInfo info, bool addPoolOnFailure = true)
    {
        var tag = poolObject.PoolTag;
        if (pools.ContainsKey(tag))
        {
            IPoolObject result = pools[tag].Pop();
            info = PoolPopInfo.done;
            if (result == null)
            {
                result = ForcePop(pools[tag].Prefab, pools[tag].Root);
                info = (PoolPopInfo.done|PoolPopInfo.force);
            }          
            return result;             
        }
        else
        {
            if (addPoolOnFailure)
            {
                if (AddPool(poolObject))
                {
                    return Pop(poolObject, out info, addPoolOnFailure);
                }
            }
            info = PoolPopInfo.failure;
            return null;
        }
    }
    public IPoolObject Pop(IPoolObject poolObject, bool addPoolOnFailure = true)
    {
        PoolPopInfo info;
        return Pop(poolObject, out info, addPoolOnFailure);
    }
    private IPoolObject ForcePop(string prefab, Transform parent)
    {
        GameObject go = Instantiate(Resources.Load<GameObject>(prefab));
        go.name += go.GetInstanceID().ToString();
        go.transform.parent = parent;
        IPoolObject result = go.GetComponent<IPoolObject>();
        result.OnPop();
        return result;
    }
    private GameObject CreateRoot(string _rootName)
    {
        GameObject root = new GameObject();
        root.transform.parent = transform;
        root.name = _rootName;
        return root;
    }
}