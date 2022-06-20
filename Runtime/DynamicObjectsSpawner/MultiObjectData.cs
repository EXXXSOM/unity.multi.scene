using UnityEngine;
using System;

[Serializable]
public class MultiObjectData
{
    [NonSerialized] private MultiObject _multiObject;

    [SerializeField] private MultiObjectReference _metadata;
    [SerializeField] public string _prefabName;
    [SerializeField] private float[] _position;
    [SerializeField] private float[] _rotation;
    [SerializeField] private bool _isActive;

    public MultiObject MultiObject => _multiObject;
    public string PrefabName => _prefabName;
    public MultiObjectReference Metadata => _metadata;
    public float[] PositionArray => _position;
    public float[] RotationArray => _rotation;
    public bool IsActive => _isActive;
    public Vector3 Position => new Vector3(_position[0], _position[1], _position[2]);
    public Quaternion Rotation => new Quaternion(_rotation[0], _rotation[1], _rotation[2], _rotation[3]);

    public MultiObjectData(string namePrefab, MultiObject multiObject, MultiObjectReference metadata)
    {
        _position = new float[3];
        _rotation = new float[4];
        _isActive = multiObject.gameObject.activeSelf;
        _multiObject = multiObject;
        _metadata = metadata;
        _prefabName = namePrefab;
    }

    public MultiObjectData(MultiObjectData multiObjectData)
    {
        _multiObject = multiObjectData.MultiObject;
        _metadata = multiObjectData.Metadata;
        _prefabName = multiObjectData.PrefabName;
        _position = multiObjectData.PositionArray;
        _rotation = multiObjectData.RotationArray;
        _isActive = multiObjectData.IsActive;
    }

    public void Update()
    {
        _position[0] = _multiObject.transform.position.x;
        _position[1] = _multiObject.transform.position.y;
        _position[2] = _multiObject.transform.position.z;
        _rotation[0] = _multiObject.transform.rotation.x;
        _rotation[1] = _multiObject.transform.rotation.y;
        _rotation[2] = _multiObject.transform.rotation.z;
        _rotation[3] = _multiObject.transform.rotation.w;
        _isActive = _multiObject.gameObject.activeSelf;
    }

    public void SetMultiObjectInstance(MultiObject multiObject)
    {
        _multiObject = multiObject;
    }

    public void SetupMetadata(MultiObjectReference metadata)
    {
        _metadata = metadata;
    }
}