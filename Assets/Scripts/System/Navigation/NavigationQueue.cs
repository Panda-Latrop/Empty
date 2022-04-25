using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class NavigationRequest
{
    protected System.Action<bool, NavMeshPath> request;
    protected Vector3 from, to;

    public Vector3 From => from;
    public Vector3 To => to;

    public NavigationRequest(System.Action<bool, NavMeshPath> request)
    {
        this.from = Vector3.zero;
        this.to = Vector3.zero;
        this.request = request;
    }
    public void SetPoints(Vector3 from, Vector3 to)
    {
        this.from = from;
        this.to = to;
    }
    public void ReturnRequest(bool hasPath, NavMeshPath path)
    {
        request.Invoke(hasPath, path);
    }
}
public class NavigationQueue : MonoBehaviour
{
    protected Queue<NavigationRequest> requests = new Queue<NavigationRequest>();
    protected int maxRequestPreFrame = 2, currentRequest = 0;

    public bool RequestPath(NavigationRequest request)
    {
        if (!requests.Contains(request))
        {
            requests.Enqueue(request);
            enabled = true;
            return true;
        }
        return false;
    }

    protected void Update()
    {
        for (currentRequest = 0; currentRequest < maxRequestPreFrame && currentRequest < requests.Count; currentRequest++)
        {
            bool hasPath = false;
            NavigationRequest request = requests.Dequeue();
            NavMeshPath path = new NavMeshPath();
            NavMeshHit fromHit, toHit;

            if (NavMesh.SamplePosition(request.From, out fromHit, 2.0f, (1 << 0)) &&
                NavMesh.SamplePosition(request.To, out toHit, 2.0f, (1 << 0)))
                hasPath = NavMesh.CalculatePath(fromHit.position, toHit.position, (1 << 0), path);
            request.ReturnRequest(hasPath, path);
        }
        if (requests.Count <= 0)
            enabled = false;
    }
}
