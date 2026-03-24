using UnityEngine;
using System.Collections.Generic;


public class GridScript : MonoBehaviour
{
    public bool DisplayGridGizmos;

    private Node[,] grid;

    [SerializeField] private LayerMask unwalkableMask;
    [SerializeField] private Vector2 worldGridSize;

    [SerializeField] private float worldNodeRadius;
    private float worldNodeDiameter;

    public int MaxSize { get {  return grid.GetLength(0) * grid.GetLength(1); } }

    private void Awake()
    {
        worldNodeDiameter = worldNodeRadius * 2;

        int gridRows = Mathf.RoundToInt(worldGridSize.x / worldNodeDiameter);
        int gridCols = Mathf.RoundToInt(worldGridSize.y / worldNodeDiameter);
        
        grid = CreateGrid(gridRows, gridCols);
    }

    private Node[,] CreateGrid(int rows, int cols)
    {
        Node[,] grid = new Node[rows, cols];
        Vector3 worldGridBottomLeft = transform.position - Vector3.right * worldGridSize.x / 2 - Vector3.forward * worldGridSize.y / 2;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Vector3 worldNodePos = worldGridBottomLeft + Vector3.right * (worldNodeDiameter * i + worldNodeRadius) 
                                                           + Vector3.forward * (worldNodeDiameter * j + worldNodeRadius);

                bool isWalkable = !Physics.CheckSphere(worldNodePos, worldNodeRadius, unwalkableMask);

                grid[i, j] = new Node(i, j, worldNodePos, isWalkable);
            }
        }

        return grid;
    }

    public Node GetNodeFromWorldPos(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + worldGridSize.x / 2) / worldGridSize.x;
        percentX = Mathf.Clamp01(percentX);

        float percentY = (worldPosition.z + worldGridSize.y / 2) / worldGridSize.y;
        percentY = Mathf.Clamp01(percentY);

        int row = Mathf.RoundToInt((grid.GetLength(0) - 1) * percentX);
        int col = Mathf.RoundToInt((grid.GetLength(1) - 1) * percentY);

        return grid[row, col];
    }

    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }

                var neighborRow = node.Row + i;
                var neighborCol = node.Col + j;

                if (neighborRow >= 0 && neighborRow < grid.GetLength(0)  && neighborCol >=
                    0 && neighborCol < grid.GetLength(1))
                {
                    neighbors.Add(grid[neighborRow, neighborCol]);
                }
            }
        }

        return neighbors;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(worldGridSize.x, 1, worldGridSize.y));

        if (grid != null && DisplayGridGizmos)
        {
            foreach (Node node in grid)
            {
                Gizmos.color = node.IsWalkable ? Color.white : Color.red;                
                Gizmos.DrawCube(node.WorldPos, Vector3.one * (worldNodeDiameter - 0.1f));
            }
        }
    }
}
