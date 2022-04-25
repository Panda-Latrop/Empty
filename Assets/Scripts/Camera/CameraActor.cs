using SimpleJSON;
using System.Collections;
using UnityEngine;

public class CameraActor : Actor
{
    [SerializeField]
    protected new Camera camera;
    protected Vector3 lookRotation;


    public Camera Instance => camera;
    public Vector3 LookRotation { get => lookRotation; set => lookRotation = value; }

    public void LookRotate(Vector3 rotate)
    {
        lookRotation = Utility.ClampRotation(lookRotation + rotate, -90.0f, 90.0f);
    }


    protected virtual void Update()
    {
        transform.localRotation = Quaternion.Euler(lookRotation);
    }

    public override JSONObject Save(JSONObject jsonObject)
    {
        base.Save(jsonObject);
        return jsonObject;
    }
    public override JSONObject Load(JSONObject jsonObject)
    {
        base.Load(jsonObject);
        return jsonObject;
    }
}
