using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStart : MonoBehaviour
{
    [System.Serializable]
    protected struct StartPointStruct
    {
        public Vector3 position;
        public Quaternion rotation;

    }


    [SerializeField]
    protected List<StartPointStruct> points = new List<StartPointStruct>();
    public void GetPoint(int index, out Vector3 position, out Quaternion rotation)
    {
        index--;
        if (index >= 0 && index < points.Count)
        {
            position = points[index].position;
            rotation = points[index].rotation;
        }
        else
        {
            position = transform.position;
            rotation = transform.rotation;
        }
    }

    protected void OnDrawGizmos()
    {

        if (!Application.isPlaying)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(transform.position + (transform.rotation * Vector3.forward * 0.5f), 0.25f);
            Gizmos.DrawSphere(transform.position, 0.5f);       
            for (int i = 0; i < points.Count; i++)
            {
                Gizmos.DrawSphere(points[i].position + (points[i].rotation * Vector3.forward * 0.5f), 0.25f);
                Gizmos.DrawSphere(points[i].position, 0.5f);              
            }
        }
    }
}
