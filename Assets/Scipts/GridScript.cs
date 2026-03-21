using System;
using UnityEngine;

public class Grid : MonoBehaviour
{
    private Node[,] grid;

    [SerializeField] private LayerMask unwalkableMask;
    [SerializeField] private Vector2 worldGridSize;

    [SerializeField] private float worldNodeRadius;
    private float worldNodeDiameter;

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
        Vector3 worldGridTopLeft = transform.position - Vector3.right * worldGridSize.x / 2 + Vector3.forward * worldGridSize.y / 2;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Vector3 worldNodePos = worldGridTopLeft + Vector3.right * (worldNodeDiameter * i + worldNodeRadius) - Vector3.forward * (worldNodeDiameter * j + worldNodeRadius);
                bool isWalkable = !Physics.CheckSphere(worldNodePos, worldNodeRadius, unwalkableMask);

                grid[i, j] = new Node(i, j, worldNodePos, isWalkable);
            }
        }

        return grid;
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(worldGridSize.x, 1, worldGridSize.y));

        if (grid != null)
        {
            foreach (Node node in grid)
            {
                Gizmos.color = node.isWalkable ? Color.white : Color.red;
                Gizmos.DrawCube(node.worldPos, Vector3.one * worldNodeDiameter);
            }
        }
    }
}
