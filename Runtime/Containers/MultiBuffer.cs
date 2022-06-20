using System.Collections.Generic;

public class MultiBuffer<K,T>
{
    public bool IsUsed = false;
    public readonly Dictionary<K,T> Elements;

    public MultiBuffer(int capacity)
    {
        Elements = new Dictionary<K, T>(capacity);
    }

    public void AddElement(K key, T element)
    {
        Elements.Add(key, element);
    }

    public void Release()
    {
        Elements.Clear();
        IsUsed = false;
    }
}
