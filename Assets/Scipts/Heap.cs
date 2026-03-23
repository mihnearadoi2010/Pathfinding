using System;


public class Heap<T> where T : IHeapItem<T>
{
    private T[] items;
    public int Length { get; private set; }

    public Heap(int maxSize)
    {
        items = new T[maxSize];
    }

    public void Add(T item)
    {
        item.HeapIndex = Length;
        items[Length] = item;

        Length++;
        SortUp(item);
    }

    public T GetFirst()
    {
        var firstItem = items[0];
        items[0] = items[Length - 1];
        items[0].HeapIndex = 0;

        Length--;
        SortDown(items[0]);
       
        return firstItem;
    }

    public bool Contains(T item)
    {
        if (item.HeapIndex < Length)
        {
            return Equals(items[item.HeapIndex], item);
        }
        return false;
    }

    public void Clear()
    {
        Length = 0;
    }

    private void SortDown(T item)
    {
        while (true)
        {
            var leftChildIndex = items[item.HeapIndex].HeapIndex * 2 + 1;
            var rightChildIndex = items[item.HeapIndex].HeapIndex * 2 + 2;
            int swapIndex;

            if (leftChildIndex < Length)
            {
                swapIndex = leftChildIndex;
                if (rightChildIndex < Length && items[rightChildIndex].CompareTo(items[leftChildIndex]) > 0)
                {
                    swapIndex = rightChildIndex;
                }
            }
            else
            {
                return;
            }

            if (item.CompareTo(items[swapIndex]) < 0)
            {
                Swap(item, items[swapIndex]);
            }
            else
            {
                return;
            }
        }
    }

    private void SortUp(T item)
    {
        while (true)
        {
            var parent = items[(item.HeapIndex - 1) / 2];

            if (item.CompareTo(parent) > 0)
            {
                Swap(item, parent);
            }
            else
            {
                break;
            }
        }
    }

    private void Swap(T itemA, T itemB)
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;

        var itemIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemIndex;
    }
}

public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex { get; set; }
}
