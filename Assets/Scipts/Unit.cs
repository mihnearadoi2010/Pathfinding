using System.Collections;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private bool ShowPathGizmos;
    [SerializeField] private Transform target;
    private PlayerScript player;

    private const float speed = 20;
    private Vector3[] path;

    private int currentWaypointIndex = 0;
    private Coroutine followCoroutine;

    private const float requestCooldown = 0.5f;
    private float lastRequestTime;

    private void Start()
    {
        player = FindAnyObjectByType<PlayerScript>();
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
    }

    private void Update()
    {
        GetPlayerPosition();
    }

    private void GetPlayerPosition()
    {
        if (player.Velocity.sqrMagnitude > 0.01 && Time.time - lastRequestTime > requestCooldown)
        {
            PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
            lastRequestTime = Time.time;
        }
        
    }

    private void OnPathFound(Vector3[] newPath, bool hasFoundPath)
    {
        if (!hasFoundPath)
        {
            return;
        }

        path = newPath;

        if (followCoroutine != null)
        {
            StopCoroutine(followCoroutine);
        }
        
        followCoroutine = StartCoroutine(FollowPath());
    }

    private IEnumerator FollowPath()
    {
        for (int i = 0; i < path.Length; i++)
        {
            Vector3 currentWaypoint = new Vector3(path[i].x, transform.position.y, path[i].z);
            currentWaypointIndex = i;

            while (true)
            {
                if (Vector3.Distance(transform.position, currentWaypoint) < 0.1)
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
