using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
#endif

public static class MultiObjectManager
{
    private static CacheMultiObjectContainer currentSceneCacheMultiObjectContainer;
    public static CacheMultiObjectContainer GetCurrentSceneCacheMultiObjectContainer { get { return currentSceneCacheMultiObjectContainer; } }

    private static ObjectStateLoader _objectStateLoader;
    private static MultiObjectLoader _multiObjectLoader;
    public static ObjectStateLoader ObjectStateLoader => _objectStateLoader;
    public static MultiObjectLoader MultiObjectLoader => _multiObjectLoader;

    //CACHE FOR SAVE
    private static Dictionary<int, CacheMultiObjectContainer> _cacheMultiObjectContainer;

    private static bool initialized = false;
    public static List<MultiSceneObserver> needLoadObserver = new List<MultiSceneObserver>(2);

    public static void Bootstrap(MOData data)
    {
        if (initialized) return;

        initialized = true;
        LoadData(data);

        SceneManager.activeSceneChanged += SetActiveSceneMultiObjectContainer;
        SceneManager.sceneLoaded += SceneLoaded;
        SceneManager.sceneUnloaded += SceneUnload;

        _multiObjectLoader.Bootstrap();
        LoadObserverQueue();
    }

    private static void LoadObserverQueue()
    {
        for (int i = 0; i < needLoadObserver.Count; i++)
        {
            LoadMultiScene(needLoadObserver[i]);
        }
        needLoadObserver.Clear();
    }

    public static void Dispose()
    {
        SceneManager.activeSceneChanged -= SetActiveSceneMultiObjectContainer;
        SceneManager.sceneLoaded -= SceneLoaded;
        SceneManager.sceneUnloaded -= SceneUnload;
    }

    public static Dictionary<int, CacheMultiObjectContainer> GetMultiSceneData()
    {
        return _cacheMultiObjectContainer;
    }

    #region GetCachedMultiObjectContainer
    public static CacheMultiObjectContainer GetCachedMultiObjectContainer(string sceneName)
    {
        int sceneHash = sceneName.GetHashCode();

        if (_cacheMultiObjectContainer.ContainsKey(sceneHash))
        {
            return _cacheMultiObjectContainer[sceneHash];
        }
        else
        {
            CacheMultiObjectContainer cacheContainer = new CacheMultiObjectContainer(sceneHash);
            _cacheMultiObjectContainer.Add(sceneHash, cacheContainer);
            return cacheContainer;
        }
    }

    public static CacheMultiObjectContainer GetCachedMultiObjectContainer(int sceneHash)
    {
        if (_cacheMultiObjectContainer.ContainsKey(sceneHash))
        {
            return _cacheMultiObjectContainer[sceneHash];
        }
        else
        {
            CacheMultiObjectContainer cacheContainer = new CacheMultiObjectContainer(sceneHash);
            _cacheMultiObjectContainer.Add(sceneHash, cacheContainer);
            return cacheContainer;
        }
    }
    #endregion

    public static bool CheckContainer(string sceneName)
    {
        int sceneHash = sceneName.GetHashCode();
        return _cacheMultiObjectContainer.ContainsKey(sceneHash);
    }

    public static bool CheckContainer(int sceneHash)
    {
        return _cacheMultiObjectContainer.ContainsKey(sceneHash);
    }

    public static void LoadMultiScene(MultiSceneObserver observer)
    {
        if (initialized == false)
        {
            needLoadObserver.Add(observer);
            return;
        }

        CacheMultiObjectContainer containerForScene = GetCachedMultiObjectContainer(observer.gameObject.scene.name);
        observer.MultiObjectContainer = containerForScene;

        containerForScene.Load(observer);

        if (observer.IsLoadMultiObjects)
        {
            _multiObjectLoader.RespawnMultiObjectsFromContainer(containerForScene);
        }

        if (observer.IsloadStates)
        {
            _objectStateLoader.LoadStates(observer, containerForScene);
        }

        if (containerForScene.LoadedSceneMultiObjects == false)
        {
            MultiObject multiObject = null;
            for (int i = 0; i < observer.GetAllStaticMultiObject.Count; i++)
            {
                multiObject = observer.GetAllStaticMultiObject[i];
                MultiObjectLoader.Spawn(multiObject, multiObject.transform.position, containerForScene);
                multiObject.gameObject.SetActive(false);
            }
            containerForScene.LoadedSceneMultiObjects = true;
        }
        else
        {
            for (int i = 0; i < observer.GetAllStaticMultiObject.Count; i++)
            {
                observer.GetAllStaticMultiObject[i].gameObject.SetActive(false);
            }
        }
    }

    private static void SetActiveSceneMultiObjectContainer(Scene oldScene, Scene newScene)
    {
        int sceneHash = newScene.name.GetHashCode();

        if (_cacheMultiObjectContainer.ContainsKey(sceneHash))
        {
            Debug.Log("[MultiObjectManager]: Установленая новая активная сцена <" + newScene.name + ">");
            currentSceneCacheMultiObjectContainer = _cacheMultiObjectContainer[sceneHash];
        }
    }

    //Загрузить все существующие контейнеры
    private static void LoadData(MOData data)
    {
        if (data == null)
        {
            CreateEmptyDate();
        }
        else
        {
            //Есть данные
            _cacheMultiObjectContainer = data.CacheMultiObjectContainer;
            _objectStateLoader = new ObjectStateLoader();
            _multiObjectLoader = new MultiObjectLoader(data.GlobalIndex);
        }

        Debug.Log("[MultiObjectManager]: В память загружено <" + _cacheMultiObjectContainer.Count + "> контейнеров сцен.");

        int sceneHash = SceneManager.GetActiveScene().name.GetHashCode();

        if (_cacheMultiObjectContainer.ContainsKey(sceneHash) == false)
        {
            CacheMultiObjectContainer cacheContainer = new CacheMultiObjectContainer(sceneHash);
            _cacheMultiObjectContainer.Add(sceneHash, cacheContainer);
            Debug.Log("[MultiObjectManager]: Создан контейнер для сцены <" + SceneManager.GetActiveScene().name + ">.");
        }


#if UNITY_EDITOR
        long size = 0;
        using (Stream s = new MemoryStream())
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(s, _cacheMultiObjectContainer);
            size = s.Length;
            Debug.Log("[MultiObjectManager] Размер кеша MultiScene: " + size + " байт.");
        }
#endif
    }

    private static void SceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        int sceneHash = scene.name.GetHashCode();

        Debug.Log("Загружена сцена " + scene.name);
        if (_cacheMultiObjectContainer.ContainsKey(sceneHash) == false)
        {
            CacheMultiObjectContainer cacheContainer = new CacheMultiObjectContainer(sceneHash);
            _cacheMultiObjectContainer.Add(sceneHash, cacheContainer);
            Debug.Log("[MultiObjectManager]: Создан контейнер для сцены <" + scene.name + ">.");
        }
    }

    private static void SceneUnload(Scene scene)
    {
        Debug.Log("Выгружена сцена " + scene.name);
        CacheMultiObjectContainer containerForScene = GetCachedMultiObjectContainer(scene.name);
        containerForScene.Unload();
    }

    //public static async void SaveCachedData()
    //{
    //    Debug.Log("[MultiObjectManager] Начало выполнения сохранения!");
    //    await Task.Run(() => SaveProcess());
    //    Debug.Log("[MultiObjectManager] Сохранение окончено успешно!");
    //}

    //private static void SaveProcess()
    //{
    //    Debug.Log("[MultiObjectManager] Процесс сохранения!");
    //    foreach (var cachedContainer in CacheMultiObjectContainer)
    //    {
    //        if (cachedContainer.Value.Changed)
    //        {
    //            MultiObjectContainer container = allFinedMultiObjectContainers[cachedContainer.Key];

    //            container.DisabledMultiObjectsIndexes = cachedContainer.Value.DisabledMultiObjectsIndexes;
    //            container.SavedMultiObjects = cachedContainer.Value.SavedMultiObjects;
    //            container.SavedObjectStates = cachedContainer.Value.SavedObjectStates;
    //        }
    //    }
    //}

    private static void CreateEmptyDate()
    {
        _cacheMultiObjectContainer = new Dictionary<int, CacheMultiObjectContainer>(3);
        _objectStateLoader = new ObjectStateLoader();
        _multiObjectLoader = new MultiObjectLoader();
    }

    public static MOData SaveData()
    {
        MOData data = new MOData();
        data.GlobalIndex = MultiObjectLoader.NextIndex;
        data.CacheMultiObjectContainer = _cacheMultiObjectContainer;
        return data;
    }

    [System.Serializable]
    public class MOData
    {
        public int GlobalIndex = 0; 
        public Dictionary<int, CacheMultiObjectContainer> CacheMultiObjectContainer;
    }
}
