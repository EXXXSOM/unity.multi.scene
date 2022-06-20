using UnityEngine;
using System;

[Serializable]
public class MultiObjectData
{
    [NonSerialized] public MultiObject MultiObject;

    [SerializeField] private MultiObjectReference _multiObjectReference;
    [SerializeField] public string _prefabName;
    [SerializeField] private float[] _position = new float[3];

    public int Index { get { return _multiObjectReference.Index; } }
    public int MySceneHash { get { return _multiObjectReference.SceneHash; } }
    public string PrefabName { get { return _prefabName; } }
    public Vector3 Position => new Vector3(_position[0], _position[1], _position[2]);

    public MultiObjectData(Vector3 position, string namePrefab, int index, int sceneHash)
    {
        _multiObjectReference = new MultiObjectReference(index, sceneHash);
        _position[0] = position.x;
        _position[1] = position.y;
        _position[2] = position.z;
        _prefabName = namePrefab;
    }
}