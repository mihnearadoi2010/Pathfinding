using UnityEngine;

public class Node : IHeapItem<Node>
{
    public int Row { get; }
    public int Col { get; }

    public Vector3 WorldPos { get; }
    public bool IsWalkable { get; }

    public int GCost { get; set; } //distance from start node
    public int HCost { get; set; } //distance from end node
    public int FCost { get { return GCost + HCost; } }

    public Node Parent { get; set; }
    public int HeapIndex { get; set; }

    public Node(int row, int col, Vector3 worldPos, bool isWalkable)
    {
        Row = row;
        Col = col;
        WorldPos = worldPos;
        IsWalkable = isWalkable;
    }

    public int GetDistance(Node otherNode)
    {
        int rowDistance = Mathf.Abs(otherNode.Row - this.Row);
        int colDistance = Mathf.Abs(otherNode.Col - this.Col);

        if (rowDistance < colDistance)
        {
            return rowDistance * 14 + (colDistance - rowDistance) * 10;
        }
        return colDistance * 14 + (rowDistance - colDistance) * 10;
    }

    public int CompareTo(Node other)
    {
        int result = this.FCost.CompareTo(other.FCost);

        if (result == 0)
        {
            result = this.HCost.CompareTo(other.HCost);
        }
        return -result;
    }
}
