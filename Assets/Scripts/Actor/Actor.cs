using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Actor : MonoBehaviour, ISaveable
{
    [SerializeField]
    protected string path;
    [SerializeField]
    protected string saveTag;
    [SerializeField]
    protected string specifier = "base";
#if UNITY_EDITOR

    [ContextMenu("Set Path From Selected In Floder")]
    private void AutoPath()
    {
        string path = AssetDatabase.GetAssetPath(Selection.gameObjects[0]);
        path = path.Replace("Assets/Resources/", string.Empty);
        path = path.Replace(".prefab", string.Empty);
        if (path.Length > 0)
        this.path = path;
        Debug.Log("prefab path:" + path);
    }
#endif
    public string Path => path;
    public string Specifier => specifier;
    public string SaveTag { get => saveTag; set => saveTag = value; }
    public GameObject GameObject => gameObject;
    public Transform Transform => transform;
    public virtual Vector3 Center => transform.position;
    public virtual Bounds Bounds => new Bounds(transform.position, Vector3.one);
    public virtual void SetPosition(Vector3 position) => transform.position = position;
    public virtual void SetRotation(Quaternion rotation) => transform.rotation = rotation;
    public virtual void SetTransform(Vector3 position, Quaternion rotation) { transform.position = position; transform.rotation = rotation; }
    public virtual void SetScale(Vector3 scale) => transform.localScale = scale;
    public virtual void SetParent(Transform parent) => transform.parent = parent;
    public virtual JSONObject Save( JSONObject jsonObject)
    {
        jsonObject.Add("name", new JSONString(gameObject.name));
        jsonObject.Add("prefab", new JSONString(path));
        jsonObject.Add("enabled", new JSONBool(enabled));
        jsonObject.Add("saveTag", new JSONString(saveTag));
        SaveSystem.GameObjectSave(jsonObject, gameObject);
        return jsonObject;
    }
    public virtual JSONObject Load( JSONObject jsonObject)
    {
        gameObject.name = jsonObject["name"];
        enabled = jsonObject["enabled"].AsBool;
        saveTag = jsonObject["saveTag"];
        SaveSystem.GameObjectLoad(jsonObject, gameObject);
        return jsonObject;
    }
}
