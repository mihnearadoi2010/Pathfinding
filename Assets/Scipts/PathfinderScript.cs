using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class PathfinderScript : MonoBehaviour
{
    private GridScript grid;
    private PathRequestManager pathRequestManager;

    private Heap<Node> open;
    HashSet<Node> closed;

    private void Awake()
    {
        grid = GetComponent<GridScript>();
        pathRequestManager = GetComponent<PathRequestManager>();

        open = new Heap<Node>(grid.MaxSize);
        closed = new HashSet<Node>();
    }

    public void StartFindPath(Vector3 startPos, Vector3 targetPos)
    {
        StartCoroutine(FindPath(startPos, targetPos));
    }

    IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.GetNodeFromWorldPos(startPos);
        Node targetNode = grid.GetNodeFromWorldPos(targetPos);

        Vector3[] pathWaypoints = new Vector3[0];
        bool hasFoundPath = false;

        if (startNode.IsWalkable && targetNode.IsWalkable)
        {
            open.Clear();
            closed.Clear();

            open.Add(startNode);

            while (open.Length > 0)
            {
                var currentNode = open.GetFirst();
                closed.Add(currentNode);

                if (currentNode == targetNode)
                {
                    hasFoundPath = true;
                    break;
                }

                foreach (var neighbor in grid.GetNeighbors(currentNode))
                {
                    if (closed.Contains(neighbor) || !neighbor.IsWalkable)
                    {
                        continue;
                    }

                    int newNeighborGCost = currentNode.GCost + neighbor.GetDistance(currentNode) + neighbor.Weight;

                    if (newNeighborGCost < neighbor.GCost || !open.Contains(neighbor))
                    {
                        neighbor.GCost = newNeighborGCost;
                        neighbor.HCost = neighbor.GetDistance(targetNode);
                        neighbor.Parent = currentNode;

                        if (!open.Contains(neighbor))
                        {
                            open.Add(neighbor);
                        }
                        else
                        {
                            open.Update(neighbor);
                        }
                    }
                }
            }
        }
        
        yield return null;

        if (hasFoundPath)
        {
            pathWaypoints = RetracePath(startNode, targetNode);
        }
        pathRequestManager.FinishProcessingPath(pathWaypoints, hasFoundPath);
    }

    private Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        var currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }

        Vector3[] pathWaypoints = SimplifyPath(path);
        Array.Reverse(pathWaypoints);
        return pathWaypoints;
    }

    private Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> pathWaypoints = new List<Vector3>();
        Vector2 oldDirection = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 newDirection = new Vector2(path[i - 1].Row - path[i].Row, path[i - 1].Col - path[i].Col);
            if (newDirection != oldDirection)
            {
                pathWaypoints.Add(path[i].WorldPos);
            }
            oldDirection = newDirection;
        }

        return pathWaypoints.ToArray();
    }
}
