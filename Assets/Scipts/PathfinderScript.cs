using UnityEngine;
using System.Collections.Generic;


public class PathfinderScript : MonoBehaviour
{
    private GridScript grid;

    private Heap<Node> open;
    HashSet<Node> closed;

    private void Awake()
    {
        grid = GetComponent<GridScript>();
        open = new Heap<Node>(grid.MaxSize);
        closed = new HashSet<Node>();
    }

    private List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.GetNodeFromWorldPos(startPos);
        Node targetNode = grid.GetNodeFromWorldPos(targetPos);

        open.Clear();
        closed.Clear();

        open.Add(startNode);

        while (open.Length > 0)
        {
            var currentNode = open.GetFirst();
            closed.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (var neighbor in grid.GetNeighbors(currentNode))
            {
                if (closed.Contains(neighbor) || !neighbor.IsWalkable)
                {
                    continue;
                }

                int newNeighborGCost = currentNode.GCost + neighbor.GetDistance(currentNode);

                if (newNeighborGCost < neighbor.GCost || !open.Contains(neighbor))
                {
                    neighbor.GCost = newNeighborGCost;
                    neighbor.HCost = neighbor.GetDistance(targetNode);
                    neighbor.Parent = currentNode;

                    if (!open.Contains(neighbor))
                    {
                        open.Add(neighbor);
                    }
                }
            }
        }

        return null;
    }

    private List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        var currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }

        path.Reverse();
        return path;
    }
}
