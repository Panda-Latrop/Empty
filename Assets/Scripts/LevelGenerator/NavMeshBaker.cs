using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class NavMeshBaker : MonoBehaviour
{
    [SerializeField]
    protected NavMeshSurface surface;
    //protected void Start()
    //{
    //    BuildNavMesh();
    //}
    [ContextMenu("Build")]
    private void BuildNavMesh()
    {
        surface.BuildNavMesh();
    }
}

