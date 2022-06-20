using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class MultiObjectLoader
{
    private int _nextGlobalIndex = 0;
    private int _startBufferCapacity = 10;
    private ContainerController _containerController;
    private Transform _parent;
    private List<MultiBuffer<int, MultiObjectData>> _buffers;
    public int NextGlobalIndex => _nextGlobalIndex;

    public MultiObjectLoader(ContainerController containerController, int nextIndex)
    {
        _nextGlobalIndex = nextIndex;
        _containerController = containerController;

        GameObject parent = new GameObject("Objects");
        SceneManager.MoveGameObjectToScene(parent, MultiSceneManager.GlobalMultiObjectScene);
        _parent = parent.transform;
    }

    public void UpdateAllMultiObjects()
    {
        foreach (var container in _containerController.GetContainers())
        {
            foreach (var multiObjectData in container.MultiObjectDatas.Values)
                multiObjectData.Update();
        }
    }

    public void LoadState(int nextGlobaIndex)
    {
        _nextGlobalIndex = nextGlobaIndex;
    }

    public void CreateBuffers(int countBuffers, int bufferCapacity)
    {
        _startBufferCapacity = bufferCapacity;
        _buffers = new List<MultiBuffer<int, MultiObjectData>>(countBuffers);

        for (int i = 0; i < countBuffers; i++)
        {
            _buffers.Add(new MultiBuffer<int, MultiObjectData>(bufferCapacity));
        }
    }

    public MultiBuffer<int, MultiObjectData> GetBuffer()
    {
        for (int i = 0; i < _buffers.Count; i++)
        {
            if (_buffers[i].IsUsed)
            {
                continue;
            }
            else
            {
                _buffers[i].IsUsed = true;
                return _buffers[i];
            }
        }

#if UNITY_EDITOR
        Debug.LogWarning("[MultiObjectLoader]: Не хватает начальных буфферов. Создаем дополнительный.");
#endif
        _buffers.Add(new MultiBuffer<int, MultiObjectData>(_startBufferCapacity));
        return _buffers[_buffers.Count - 1];
    }

    #region SPAWN_METHODS
    public async Task<MultiObject> Spawn(AssetReference prefab, Scene parentScene)
    {
        MultiObject multiObjectSpawned = await LoadInstanceAsync(prefab, _parent);
        AddMultiObjectToContainer(parentScene.name, multiObjectSpawned, prefab.editorAsset.name);
        return multiObjectSpawned;
    }

    public async Task<MultiObject> Spawn(string multiObjectPrefabName, Vector3 position, Quaternion rotation, Scene parentScene)
    {
        MultiObject multiObjectSpawned = await LoadInstanceAsync(multiObjectPrefabName, position, rotation, _parent);
        AddMultiObjectToContainer(parentScene.name, multiObjectSpawned, multiObjectPrefabName);
        return multiObjectSpawned;
    }
    public async Task<MultiObject> Spawn(string multiObjectPrefabName, Vector3 position, Quaternion rotation, MultiScene multiScene)
    {
        MultiObject multiObjectSpawned = await LoadInstanceAsync(multiObjectPrefabName, position, rotation, _parent);
        AddMultiObjectToContainer(multiScene.SceneName, multiObjectSpawned, multiObjectPrefabName);
        return multiObjectSpawned;
    }

    public async Task<MultiObject> Spawn(AssetReference prefab, Vector3 position, Quaternion rotation, Scene parentScene)
    {
        //Debug.LogError("TEST: " + prefab.SubObjectName);
        MultiObject multiObjectSpawned = await LoadInstanceAsync(prefab, position, rotation, _parent);
        AddMultiObjectToContainer(parentScene.name, multiObjectSpawned, prefab.editorAsset.name);
        return multiObjectSpawned;
    }

    public async Task<MultiObject> Spawn(AssetReference prefab, Vector3 position, Quaternion rotation, MultiScene multiScene)
    {
        Debug.LogError("TEST: " + prefab.SubObjectName);
        MultiObject multiObjectSpawned = await LoadInstanceAsync(prefab, position, rotation, _parent);
        AddMultiObjectToContainer(multiScene.SceneName, multiObjectSpawned, prefab.editorAsset.name);
        return multiObjectSpawned;
    }

    public async Task<MultiObject> Spawn(AssetReference prefab, MultiScene multiScene)
    {
        MultiObject multiObjectSpawned = await LoadInstanceAsync(prefab, _parent);
        AddMultiObjectToContainer(multiScene.SceneName, multiObjectSpawned, prefab.editorAsset.name);
        return multiObjectSpawned;
    }

    public async Task<MultiObject> SpawnInGlobalContainer(string multiObjectPrefabName, Quaternion rotation, Vector3 position)
    {
        MultiObject multiObjectSpawned = await LoadInstanceAsync(multiObjectPrefabName, position, rotation, _parent);
        AddMultiObjectToContainer(MultiSceneManager.GLOBAL_MULTI_SCENE_NAME, multiObjectSpawned, multiObjectPrefabName);
        return multiObjectSpawned;
    }

    public async Task<MultiObject> SpawnInGlobalContainer(string multiObjectPrefabName)
    {
        MultiObject multiObjectSpawned = await LoadInstanceAsync(multiObjectPrefabName, _parent);
        AddMultiObjectToContainer(MultiSceneManager.GLOBAL_MULTI_SCENE_NAME, multiObjectSpawned, multiObjectPrefabName);
        return multiObjectSpawned;
    }

    public async Task<MultiObject> SpawnInGlobalContainer(AssetReference prefab)
    {
        MultiObject multiObjectSpawned = await LoadInstanceAsync(prefab, _parent);
        AddMultiObjectToContainer(MultiSceneManager.GLOBAL_MULTI_SCENE_NAME, multiObjectSpawned, prefab.editorAsset.name);
        return multiObjectSpawned;
    }
    #endregion

    #region DESPAWN_METHODS
    public void SaveAndDespawnMultiObjectOnScene(Container container)
    {
        foreach (var multiObjectData in container.MultiObjectDatas.Values)
        {
            multiObjectData.Update();
            DespawnMultiObject(multiObjectData.MultiObject, container);
        }
    }

    private bool DespawnMultiObject(MultiObject multiObject)
    {
        Container container;
        _containerController.GetContainer(multiObject.MultiObjectReference.SceneHash, out container);
        container.RemoveMultiObjectDataAt(multiObject.MultiObjectReference.Index);
        return Addressables.ReleaseInstance(multiObject.AsyncOperationHandle);
    }

    private bool DespawnMultiObject(MultiObject multiObject, Container container)
    {
        container.RemoveMultiObjectDataAt(multiObject.MultiObjectReference.Index);
        return Addressables.ReleaseInstance(multiObject.AsyncOperationHandle);
    }
    #endregion

    private async Task<MultiObject> LoadInstanceAsync(string name, Vector3 position, Quaternion rotation, Transform parent)
    {
        //trackHandle = false. Мы можем поставить true, тем самым убрать у MultiObject.AsyncOperationHandle и при
        //выгрузки multiobject использовать не дескриптер(MultiObject.AsyncOperationHandle), а сам gameObject. 
        //Таким образам, ресурсами полностью управляет Addressables.
        AsyncOperationHandle<GameObject> asyncOperationHandle = Addressables.InstantiateAsync(name, position, rotation, parent, false);
        await asyncOperationHandle.Task;
        MultiObject multiObjectSpawned = asyncOperationHandle.Result.GetComponent<MultiObject>();
#if UNITY_EDITOR
        if (multiObjectSpawned == null)
            Debug.LogError("[MultiObjectLoader]: На префабе нет <MultiObject>.");
#endif
        multiObjectSpawned.AsyncOperationHandle = asyncOperationHandle;
        return multiObjectSpawned;
    }

    private async Task<MultiObject> LoadInstanceAsync(string name, Transform parent)
    {
        //trackHandle = false. Мы можем поставить true, тем самым убрать у MultiObject.AsyncOperationHandle и при
        //выгрузки multiobject использовать не дескриптер(MultiObject.AsyncOperationHandle), а сам gameObject. 
        //Таким образам, ресурсами полностью управляет Addressables.
        AsyncOperationHandle<GameObject> asyncOperationHandle = Addressables.InstantiateAsync(name, parent, false);
        await asyncOperationHandle.Task;
        MultiObject multiObjectSpawned = asyncOperationHandle.Result.GetComponent<MultiObject>();
#if UNITY_EDITOR
        if (multiObjectSpawned == null)
            Debug.LogError("[MultiObjectLoader]: На префабе нет <MultiObject>.");
#endif
        multiObjectSpawned.AsyncOperationHandle = asyncOperationHandle;
        return multiObjectSpawned;
    }

    private async Task<MultiObject> LoadInstanceAsync(AssetReference prefab, Vector3 position, Quaternion rotation, Transform parent)
    {
        //trackHandle = false. Мы можем поставить true, тем самым убрать у MultiObject.AsyncOperationHandle и при
        //выгрузки multiobject использовать не дескриптер(MultiObject.AsyncOperationHandle), а сам gameObject. 
        //Таким образам, ресурсами полностью управляет Addressables.
        AsyncOperationHandle<GameObject> asyncOperationHandle = Addressables.InstantiateAsync(prefab, position, rotation, parent, false);
        await asyncOperationHandle.Task;
        MultiObject multiObjectSpawned = asyncOperationHandle.Result.GetComponent<MultiObject>();
#if UNITY_EDITOR
        if (multiObjectSpawned == null)
            Debug.LogError("[MultiObjectLoader]: На префабе нет <MultiObject>.");
#endif
        multiObjectSpawned.AsyncOperationHandle = asyncOperationHandle;
        return multiObjectSpawned;
    }

    private async Task<MultiObject> LoadInstanceAsync(AssetReference prefab, Transform parent)
    {
        //trackHandle = false. Мы можем поставить true, тем самым убрать у MultiObject.AsyncOperationHandle и при
        //выгрузки multiobject использовать не дескриптер(MultiObject.AsyncOperationHandle), а сам gameObject. 
        //Таким образам, ресурсами полностью управляет Addressables.
        AsyncOperationHandle<GameObject> asyncOperationHandle = Addressables.InstantiateAsync(prefab, parent, false);
        await asyncOperationHandle.Task;
        MultiObject multiObjectSpawned = asyncOperationHandle.Result.GetComponent<MultiObject>();
#if UNITY_EDITOR
        if (multiObjectSpawned == null)
            Debug.LogError("[MultiObjectLoader]: На префабе нет <MultiObject>.");
#endif
        multiObjectSpawned.AsyncOperationHandle = asyncOperationHandle;
        return multiObjectSpawned;
    }

    private void AddMultiObjectToContainer(string multiSceneName, MultiObject multiObject, string prefabeName)
    {
        //ERROR NAME
        Container container = null;
        if (_containerController.GetContainer(multiSceneName, out container) == false)
            Debug.LogError("[MultiObjectLoader]: Сцены с таким контейнером не существует <" + multiSceneName + ">");

        MultiObjectReference multiObjectReference = new MultiObjectReference(_nextGlobalIndex, container.SceneHash);
        multiObject.SetupMetadata(multiObjectReference);
        container.AddMultiObject(multiObject, multiObjectReference, prefabeName);
        _nextGlobalIndex++;
    }

    public bool GetMultiObject(MultiObjectReference multiObjectReference, out MultiObject multiObject)
    {
        Container container = null;
        if (_containerController.GetContainer(multiObjectReference.SceneHash, out container))
        {
            return container.GetMultiObject(multiObjectReference.Index, out multiObject);
        }
        else
        {
            Debug.LogError("Container?");
            multiObject = null;
            return false;
        }
    }

    public void Despawn(MultiObject multiObject)
    {
        DespawnMultiObject(multiObject);
    }

    public void MergeRealtimeAndBakedData(Container container, BakedMultiObjectData bakedMultiObjectData)
    {
        if (container.BakedDataMerged || bakedMultiObjectData == null)
        {
            //Debug.Log("Merg canceled!");
            return;
        }

        //Debug.Log("Merging...");
        for (int i = 0; i < bakedMultiObjectData.MultiObjectDatas.Length; i++)
            container.AddMultiObject(new MultiObjectData(bakedMultiObjectData.MultiObjectDatas[i]));

        container.BakedDataMerged = true;
    }

    public async Task RespawnMultiObjectsFromContainer(Container container)
    {
        foreach (var multiObjectData in container.MultiObjectDatas.Values)
        {
            MultiObject multiObjectSpawned = await LoadInstanceAsync(multiObjectData.PrefabName, multiObjectData.Position, multiObjectData.Rotation, _parent);
            multiObjectSpawned.gameObject.SetActive(multiObjectData.IsActive);
            multiObjectData.SetMultiObjectInstance(multiObjectSpawned);
            multiObjectSpawned.SetupMetadata(multiObjectData.Metadata);
        }

        container.MergeContainerAndBuffer();
        container.Loaded();
        Debug.Log("Объекты загружены, сцены: " + container.SceneHash);
    }

    public void TransferToGlobalContainer(MultiObject multiObject)
    {
        if (multiObject == null)
            return;

        Container oldContainer = null;
        Container newContainer = null;
        _containerController.GetContainer(multiObject.MultiObjectReference.SceneHash, out oldContainer);
        _containerController.GetContainer(MultiSceneManager.GLOBAL_MULTI_SCENE_NAME, out newContainer);

        if (oldContainer == null || newContainer == null)
            Debug.LogError("Нет одного из контейнеров!");

        MultiObjectData multiobjectData = null;
        if (oldContainer.GetMultiObjectData(multiObject.MultiObjectReference.Index, out multiobjectData))
        {
            oldContainer.RemoveMultiObjectDataAt(multiObject.MultiObjectReference.Index);
            MultiObjectReference metadata = new MultiObjectReference(multiObject.MultiObjectReference.Index, newContainer.SceneHash);
            multiObject.SetupMetadata(metadata);
            multiobjectData.SetupMetadata(metadata);
            newContainer.AddMultiObject(multiobjectData);
        }
    }

    public void TransferToAnotherScene(MultiObject multiObject, Scene scene)
    {
        if (multiObject == null)
            return;

        Container oldContainer = null;
        Container newContainer = null;
        _containerController.GetContainer(multiObject.MultiObjectReference.SceneHash, out oldContainer);
        _containerController.GetContainer(scene.name, out newContainer);

        if (oldContainer == null || newContainer == null)
            Debug.LogError("Нет одного из контейнеров!");

        MultiObjectData multiobjectData = null;
        if (oldContainer.GetMultiObjectData(multiObject.MultiObjectReference.Index, out multiobjectData))
        {
            oldContainer.RemoveMultiObjectDataAt(multiObject.MultiObjectReference.Index);
            MultiObjectReference metadata = new MultiObjectReference(multiObject.MultiObjectReference.Index, newContainer.SceneHash);
            multiObject.SetupMetadata(metadata);
            multiobjectData.SetupMetadata(metadata);
            newContainer.AddMultiObject(multiobjectData);
        }
    }

    //private async Task<AsyncOperationHandle<GameObject>> GlobalLoadInstanceAsync(string name, Vector3 position, Quaternion rotation, Transform parent)
    //{
    //    return Addressables.InstantiateAsync(name, position, rotation, parent, false);
    //}

    public async Task PreloadGlobalMultiObjects()
    {
        Debug.Log("Load Global MultiObject");
        Container globalContainer = null;
        if (_containerController.GetContainer(MultiSceneManager.GLOBAL_MULTI_SCENE_NAME, out globalContainer))
        {
            Debug.Log("Load Global Count: " + globalContainer.MultiObjectDatas.Count);
            await RespawnMultiObjectsFromContainer(globalContainer);
        }
        Debug.Log("Loaded Global MultiObject!!!!");
    }

    //Убирает все установленные предметы (просто удаляет со сцены)
    public void ClearAllSpawnedMultiObjects()
    {
        foreach (var container in _containerController.ContainerDatas.Values)
        {
            if (container.IsLoaded == false)
                continue;

            foreach (var multiObjectData in container.MultiObjectDatas.Values)
            {
                Addressables.ReleaseInstance(multiObjectData.MultiObject.AsyncOperationHandle);
            }
        }
    }
}
