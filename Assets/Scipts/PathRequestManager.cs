using System;
using System.Collections.Generic;
using UnityEngine;

public class PathRequestManager : MonoBehaviour
{
    private Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    private PathRequest currentPathRequest;

    private static PathRequestManager instance;
    private PathfinderScript pathfinder;

    private bool isProcessingPath;

    private struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;

        public PathRequest(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
        {
            this.pathStart = pathStart;
            this.pathEnd = pathEnd;
            this.callback = callback;
        }
    }

    private void Awake()
    {
        instance = this;
        pathfinder = GetComponent<PathfinderScript>();
    }

    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessingNext();
    }

    private void TryProcessingNext()
    {
        if (isProcessingPath || pathRequestQueue.Count == 0)
        {
            return;
        }

        currentPathRequest = pathRequestQueue.Dequeue();
        isProcessingPath = true;
    }

    private void FinishProcessingPath(Vector3[] path, bool hasFoundPath)
    {
        currentPathRequest.callback(path, hasFoundPath);
        isProcessingPath = false;
        TryProcessingNext();
    }


}
