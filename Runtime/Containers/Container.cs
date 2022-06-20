using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class Container
{
    //Dynamic data
    [NonSerialized] private bool _isLoaded = false;
    [NonSerialized] private MultiBuffer<int, MultiObjectData> _bufferMultiObjectData;
    //Save data
    [SerializeField] private bool _bakedDataMerged = false;
    [SerializeField] private int _sceneHash = 0;
    [SerializeField] private Dictionary<int, MultiObjectData> _multiObjectDatas;
    [SerializeField] private bool[] _savedIndexStateObjects;

    public bool[] SavedIndexStateObjects => _savedIndexStateObjects;
    public Dictionary<int, MultiObjectData> MultiObjectDatas => _multiObjectDatas;
    public bool IsLoaded => _isLoaded;
    public bool BakedDataMerged { get { return _bakedDataMerged; } set { _bakedDataMerged = value; } }
    public int SceneHash => _sceneHash;

    public Container(int sceneHash, int multiObjectDataCapacity)
    {
        _sceneHash = sceneHash;
        _multiObjectDatas = new Dictionary<int, MultiObjectData>(multiObjectDataCapacity);
    }

    public void Loaded()
    {
        _isLoaded = true;
    }

    public void AddMultiObject(MultiObject multiObject, MultiObjectReference multiObjectReference, string prefabeName)
    {
        if (_isLoaded)
        {
            _multiObjectDatas.Add(multiObjectReference.Index, new MultiObjectData(prefabeName, multiObject, multiObjectReference));
        }
        else
        {
            if (_bufferMultiObjectData == null)
                _bufferMultiObjectData = MultiSceneManager.MultiObjectLoader.GetBuffer();

            _bufferMultiObjectData.AddElement(multiObjectReference.Index, new MultiObjectData(prefabeName, multiObject, multiObjectReference));
            Debug.LogError("Container not loaded!");
        }
    }

    public void MergeContainerAndBuffer()
    {
        if (_bufferMultiObjectData == null)
            return;

        foreach (var element in _bufferMultiObjectData.Elements)
            _multiObjectDatas.Add(element.Key, element.Value);

        _bufferMultiObjectData.Release();
        _bufferMultiObjectData = null;
    }

    public void AddMultiObject(MultiObjectData multiObjectData)
    {
        _multiObjectDatas.Add(multiObjectData.Metadata.Index, multiObjectData);
    }

    public bool RemoveMultiObjectDataAt(int indexMultiObject)
    {
        return _multiObjectDatas.Remove(indexMultiObject);
    }

    public bool GetMultiObject(int indexMultiObject, out MultiObject multiObject)
    {
        MultiObjectData multiObjectData = null;
        if (_multiObjectDatas.TryGetValue(indexMultiObject, out multiObjectData))
        {
            multiObject = multiObjectData.MultiObject;
            return true;
        }
        else
        {
            multiObject = null;
            return false;
        }
    }

    public void SetSavedIndexStateObjects(bool[] savedIndexStateObjects)
    {
        _savedIndexStateObjects = savedIndexStateObjects;
    }

    public bool GetMultiObjectData(int indexMultiObject, out MultiObjectData multiObjectData)
    {
        return _multiObjectDatas.TryGetValue(indexMultiObject, out multiObjectData);
    }

    public void Unload()
    {
        _isLoaded = false;
    }
}
