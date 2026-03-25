using System.Collections;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private bool ShowPathGizmos;
    [SerializeField] private Transform target;

    private float speed = 20;
    private Vector3[] path;

    private int currentWaypointIndex = 0;

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

    private IEnumerator FollowPath()
    {
        for (int i = 0; i < path.Length; i++)
        {
            Vector3 currentWaypoint = new Vector3(path[i].x, transform.position.y, path[i].z);
            currentWaypointIndex = i;

            while (true)
            {
                if (transform.position == currentWaypoint)
                {
                    break;
                }

                transform.position = Vector3.MoveTowards(transform.position,currentWaypoint , speed * Time.deltaTime);
                yield return null;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!ShowPathGizmos)
        {
            return;
        }
        Gizmos.color = Color.black;

        if (path != null)
        {
            for (int i = currentWaypointIndex; i < path.Length; i++)
            {
                Vector3 destinationPos = new Vector3(path[i].x, transform.position.y, path[i].z);
                Gizmos.DrawCube(destinationPos, Vector3.one / 2);

                if (i == currentWaypointIndex)
                {
                    Gizmos.DrawLine(transform.position, destinationPos);
                }
            }
        }
        
    }
}
