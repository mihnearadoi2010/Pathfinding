using System.Collections;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private Transform target;

    private float speed = 20;
    private Vector3[] path;

    private void Start()
    {
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
    }

    private void OnPathFound(Vector3[] newPath, bool hasFoundPath)
    {
        if (!hasFoundPath)
        {
            return;
        }

        path = newPath;
        StopCoroutine(FollowPath());
        StartCoroutine(FollowPath());
    }

    IEnumerator FollowPath()
    {
        for (int i = 0; i < path.Length; i++)
        {
            Vector3 currentWaypoint = path[i];
            while (true)
            {
                if (transform.position == currentWaypoint)
                {
                    break;
                }

                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
                yield return null;
            }
        }
    }
}
