using UnityEngine;
using System;

[Serializable]
public class MultiObject
{
    [SerializeField] private int index;
    [SerializeField] private Vector3 position;
    [SerializeField] private GameObject gameObject;

    public int Index { get { return index; } }
    public Vector3 Position { get { return position; } }
    public GameObject GameObject { get { return gameObject; } }

    public MultiObject(Vector3 position, GameObject gameObject, int index)
    {
        this.index = index;
        this.position = position;
        this.gameObject = gameObject;
    }
}