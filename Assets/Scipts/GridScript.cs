using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class GridScript : MonoBehaviour
{
    [SerializeField] private bool DisplayGridGizmos;
    private int minWeight = int.MaxValue;
    private int maxWeight = int.MinValue;

    private Node[,] grid;
    private int gridRows;
    private int gridCols;

    [SerializeField] private Vector2 worldGridSize;
    [SerializeField] private float worldNodeRadius;

    [SerializeField] private TerrainType[] terrainTypes;
    [SerializeField] private LayerMask unwalkableMask;
    [SerializeField] private int obstacleProximityWeight;
    private Dictionary<int, int> walkableTerrain = new Dictionary<int, int>();
    private LayerMask walkableMask;

    private float worldNodeDiameter;

    public int MaxSize { get {  return gridRows * gridCols; } }

    private void Awake()
    {
        worldNodeDiameter = worldNodeRadius * 2;

        gridRows = Mathf.RoundToInt(worldGridSize.x / worldNodeDiameter);
        gridCols = Mathf.RoundToInt(worldGridSize.y / worldNodeDiameter);

        foreach (var terrainType in terrainTypes)
        {
            walkableMask.value |= terrainType.TerrainMask.value;
            walkableTerrain.Add((int)Mathf.Log(terrainType.TerrainMask.value, 2), terrainType.Weight);
        }
        
        grid = CreateGrid(gridRows, gridCols);
        BlurWeightMap(3);
    }

    private Node[,] CreateGrid(int rows, int cols)
    {
        Node[,] grid = new Node[rows, cols];
        Vector3 worldGridBottomLeft = transform.position - Vector3.right * worldGridSize.x / 2 - Vector3.forward * worldGridSize.y / 2;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Vector3 worldNodePos = worldGridBottomLeft + Vector3.right * (worldNodeDiameter * j + worldNodeRadius) 
                                                           + Vector3.forward * (worldNodeDiameter * i + worldNodeRadius);

                bool isWalkable = !Physics.CheckSphere(worldNodePos, worldNodeRadius, unwalkableMask);

                int weight = 0;
                Ray ray = new Ray(worldNodePos + Vector3.up * 50, Vector3.down);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100, walkableMask))
                {
                    walkableTerrain.TryGetValue(hit.collider.gameObject.layer, out weight);
                }

                if (!isWalkable)
                {
                    weight += obstacleProximityWeight;   
                }

                grid[i, j] = new Node(i, j, worldNodePos, isWalkable, weight);
            }
        }

        return grid;
    }

    private void BlurWeightMap(int blurSize)
    {
        int kernelSize = blurSize * 2 + 1;
        int kernelExtents = (kernelSize - 1) / 2;

        int[,] horizontalWeights = new int[gridRows, gridCols];
        int[,] verticalWeights = new int[gridRows, gridCols];

        for (int col = 0; col < gridCols; col++)
        {
            for (int row = -kernelExtents; row <= kernelExtents; row++)
            {
                int sampleRow = Mathf.Clamp(row, 0, kernelExtents);
                horizontalWeights[0, col] += grid[sampleRow, col].Weight;
            }

            for (int row = 1; row < gridRows; row++)
            {
                int removeIndex = Mathf.Clamp(row - kernelExtents - 1, 0, gridRows - 1);
                int addIndex = Mathf.Clamp(row + kernelExtents, 0, gridRows - 1);

                horizontalWeights[row, col] = horizontalWeights[row - 1, col] - grid[removeIndex, col].Weight + grid[addIndex, col].Weight;
            }
        }

        for (int row = 0; row < gridRows; row++)
        {
            for (int col = -kernelExtents; col <= kernelExtents; col++)
            {
                int sampleCol = Mathf.Clamp(col, 0, kernelExtents);
                verticalWeights[row, 0] += horizontalWeights[row, sampleCol];
            }

            int blurredWeight = Mathf.RoundToInt((float)verticalWeights[row, 0] / (kernelSize * kernelSize));
            grid[row, 0].Weight = blurredWeight;

            for (int col = 1; col < gridCols; col++)
            {
                int removeIndex = Mathf.Clamp(col - kernelExtents - 1, 0, gridCols - 1);
                int addIndex = Mathf.Clamp(col + kernelExtents, 0, gridCols - 1);

                verticalWeights[row, col] = verticalWeights[row, col - 1] - horizontalWeights[row, removeIndex] + horizontalWeights[row, addIndex];

                blurredWeight = Mathf.RoundToInt((float)verticalWeights[row, col] / (kernelSize * kernelSize));
                grid[row, col].Weight = blurredWeight;

                if (blurredWeight > maxWeight)
                {
                    maxWeight = blurredWeight;
                }
                if (blurredWeight < minWeight)
                {
                    minWeight = blurredWeight;
                }
            }
        }
    }

    public Node GetNodeFromWorldPos(Vector3 worldPosition)
    {
        float percentY = (worldPosition.z + worldGridSize.y / 2) / worldGridSize.y;
        percentY = Mathf.Clamp01(percentY);

        float percentX = (worldPosition.x + worldGridSize.x / 2) / worldGridSize.x;
        percentX = Mathf.Clamp01(percentX);

        int row = Mathf.RoundToInt((gridRows - 1) * percentY);
        int col = Mathf.RoundToInt((gridCols - 1) * percentX);

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

                if (neighborRow >= 0 && neighborRow < gridRows  && neighborCol >=
                    0 && neighborCol < gridCols)
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
                var weightColorDarkness = 1.7f;
                Gizmos.color = Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(minWeight, maxWeight, node.Weight * weightColorDarkness));
                Gizmos.color = node.IsWalkable ? Gizmos.color : Color.red;                
                Gizmos.DrawCube(node.WorldPos, Vector3.one * worldNodeDiameter);
            }
        }
    }
}

[System.Serializable]
public class TerrainType
{
    public LayerMask TerrainMask;
    public int Weight;
}