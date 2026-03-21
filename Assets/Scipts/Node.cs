using UnityEngine;

public class Node
{
    public int row;
    public int col;
    public Vector3 worldPos;
    public bool isWalkable;

    public Node(int row, int col, Vector3 worldPos, bool isWalkable)
    {
        this.row = row;
        this.col = col;
        this.worldPos = worldPos;
        this.isWalkable = isWalkable;
    }
}
