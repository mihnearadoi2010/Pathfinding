using UnityEngine;

public class Node
{
    public int Row { get; }
    public int Col { get; }
    public Vector3 WorldPos { get; }
    public bool IsWalkable { get; }

    public Node(int row, int col, Vector3 worldPos, bool isWalkable)
    {
        this.Row = row;
        this.Col = col;
        this.WorldPos = worldPos;
        this.IsWalkable = isWalkable;
    }
}
