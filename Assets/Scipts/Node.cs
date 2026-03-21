using UnityEngine;

public class Node
{
    public int Row;
    public int Col;
    public Vector3 WorldPos;
    public bool IsWalkable;

    public Node(int row, int col, Vector3 worldPos, bool isWalkable)
    {
        this.Row = row;
        this.Col = col;
        this.WorldPos = worldPos;
        this.IsWalkable = isWalkable;
    }
}
