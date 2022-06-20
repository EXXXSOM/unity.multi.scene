using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CacheMultiObjectContainer
{
    [System.NonSerialized] public bool IsLoaded = false;
    [System.NonSerialized] private Transform _parent;
    [System.NonSerialized] private MultiSceneObserver _observer;
    public bool LoadedSceneMultiObjects = false;
    private int _sceneHash = 0;

    public int SceneHash => _sceneHash;
    public Transform Parent => _parent;              //Properties
    public MultiSceneObserver Observer => _observer; //Properties

    [Header("Dynamically spawned objects")]
    private Dictionary<int, MultiObjectData> _savedMultiObjects = new Dictionary<int, MultiObjectData>();
    public Dictionary<int, MultiObjectData> SavedMultiObjects => _savedMultiObjects; //Properties
    public int SavedCountSpawnedMultiObjects = 0;

    [Header("Saved object states")]
    private Dictionary<int, int> _savedObjectStates = new Dictionary<int, int>();
    public Dictionary<int, int> SavedObjectStates => _savedObjectStates; //Properties

    public CacheMultiObjectContainer(int sceneHash)
    {
        _sceneHash = sceneHash;
    }

    public void AddMultiObject(MultiObjectData multiObjectData)
    {
        if (IsLoaded)
            SavedCountSpawnedMultiObjects++;

        _savedMultiObjects.Add(multiObjectData.Index, multiObjectData);
        //_nextIndex++;
    }

    public void RemoveMultiObjectAt(int index)
    {
        SavedCountSpawnedMultiObjects--;
        _savedMultiObjects.Remove(index);
    }

    public MultiObject GetMultiObject(int index)
    {
        return _savedMultiObjects[index].MultiObject;
    }

    public MultiObjectData GetMultiObjectData(int index)
    {
        return _savedMultiObjects[index];
    }

    public void AddObjectState(int indexObject, int value)
    {
        if (_savedObjectStates.ContainsKey(indexObject))
        {
            _savedObjectStates[indexObject] = value;
        }
        else
        {
            _savedObjectStates.Add(indexObject, value);
        }
    }

    public void RemoveObjectState(int indexObject)
    {
        if (_savedObjectStates.ContainsKey(indexObject))
        {
            _savedObjectStates.Remove(indexObject);
        }
    }

    public void Load(MultiSceneObserver observer)
    {
        _observer = observer;
        _parent = observer.transform;
        SavedCountSpawnedMultiObjects = SavedMultiObjects.Count;
        IsLoaded = true;
    }

    public void Unload()
    {
        IsLoaded = false;
        _parent = null;
        _observer = null;
    }

    public void Clear()
    {
        LoadedSceneMultiObjects = false;
        _savedMultiObjects.Clear();
        //_nextIndex = 0;
        SavedCountSpawnedMultiObjects = 0;
        _savedObjectStates.Clear();
    }
}
