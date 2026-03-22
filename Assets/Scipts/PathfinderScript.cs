using UnityEngine;
using System.Collections.Generic;

public class PathfinderScript : MonoBehaviour
{
    GridScript grid;

    [SerializeField] private Transform seeker;
    [SerializeField] private Transform target;

    private void Awake()
    {
        grid = GetComponent<GridScript>();
    }

    private void Update()
    {
        grid.path = FindPath(seeker.transform.position, target.transform.position);
    }

    private List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.GetNodeFromWorldPos(startPos);
        Node targetNode = grid.GetNodeFromWorldPos(targetPos);

        List<Node> open = new List<Node>();
        HashSet<Node> closed = new HashSet<Node>();

        open.Add(startNode);

        while (open.Count > 0)
        {
            var currentNode = GetCheapestNode(open);
            open.Remove(currentNode);
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

    private Node GetCheapestNode(List<Node> nodeList)
    {
        var cheapestNode = nodeList[0];

        foreach (var node in nodeList)
        {
            if (node.FCost < cheapestNode.FCost || (node.FCost == cheapestNode.FCost && node.HCost < cheapestNode.HCost))
            {
                cheapestNode = node;
            }
        }

        return cheapestNode;
    }
}
