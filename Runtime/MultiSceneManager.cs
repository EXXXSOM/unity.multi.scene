using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public static class MultiSceneManager
{
    public const string NAME_FILE_SETTINGS = "MultiSceneSettings";
    public const string GLOBAL_MULTI_SCENE_NAME = "MultiObjects"; //GLOBAL_MULTI_OBJECT_CONTAINER
    public static Scene GlobalMultiObjectScene { get; private set; }
    public static bool AutoLoadMultiScene => _multiSceneSettings.AutoLoadMultiScene;
    public static ObjectStateLoader ObjectStateLoader => _objectStateLoader;
    public static MultiObjectLoader MultiObjectLoader => _multiObjectLoader;

    private static bool _initialized = false;
    private static ObjectStateLoader _objectStateLoader;
    private static MultiObjectLoader _multiObjectLoader;
    private static ContainerController _containerController;
    private static MultiSceneSettings _multiSceneSettings;
    private static SaveObjectStateAdapter _saveAdapter;
    private static LoadObjectStateAdapter _loadAdapter;
    private static List<MultiScene> _needLoadMultiSceneQueue = new List<MultiScene>(3);
    private static List<MultiScene> _loadedMultiScene = new List<MultiScene>(7);
    private static MultiScene _globalMultiScene;

    //DATA
    private static Dictionary<string, object> _savedStateData;

    public static void Bootstrap(MOData data = null)
    {
        if (_initialized)
            return;

        LoadSettings();
        GlobalMultiObjectScene = SceneManager.CreateScene(GLOBAL_MULTI_SCENE_NAME, new CreateSceneParameters(LocalPhysicsMode.None));
        _savedStateData = new Dictionary<string, object>();
        _saveAdapter = new SaveObjectStateAdapter(_savedStateData);
        _loadAdapter = new LoadObjectStateAdapter(_savedStateData);
        CreateEmptyData();
        _initialized = true;
    }

    public async static void LoadMultiScene(MultiScene multiScene)
    {
        if (_initialized == false)
        {
            _needLoadMultiSceneQueue.Add(multiScene);
            return;
        }
        else
        {
            _loadedMultiScene.Add(multiScene);
        }

        Container container = _containerController.GetOrCteateContainer(multiScene.SceneName);
        _multiObjectLoader.MergeRealtimeAndBakedData(container, multiScene.BakedData);
        await _multiObjectLoader.RespawnMultiObjectsFromContainer(container);
        _objectStateLoader.LoadStates(multiScene, container);
        Debug.Log("[MultiObjectManager]: Loaded scene - <" + multiScene.SceneName + ">!");
    }

    public static void UnloadMultiScene(string sceneName)
    {
        MultiScene multiScene = null;
        for (int i = 0; i < _loadedMultiScene.Count; i++)
        {
            if (_loadedMultiScene[i].SceneName == sceneName)
            {
                multiScene = _loadedMultiScene[i];
                _loadedMultiScene.RemoveAt(i);
                break;
            }
        }

        if (multiScene == null)
        {
            Debug.LogError("[MultiObjectManager]: Сцена не найдена - " + sceneName);
            return;
        }

        Container container = null;
        if (_containerController.GetContainer(sceneName, out container))
        {
            //TODO Сохранять в несколько кадров
            _multiObjectLoader.SaveAndDespawnMultiObjectOnScene(container);
            _objectStateLoader.SaveStateObjects(multiScene, container);
            container.Unload();
        }
        Debug.Log("[MultiObjectManager]: Saved and unloaded scene container - " + sceneName);
    }

    public static MOData SaveData()
    {
        MOData data = new MOData();
        _multiObjectLoader.UpdateAllMultiObjects();
        _objectStateLoader.SaveAllStates(_loadedMultiScene);

        //Save data
        data.GlobalIndex = MultiObjectLoader.NextGlobalIndex;
        data.ContainerDatas = _containerController.ContainerDatas;
        data.SavedStateData = _savedStateData;
        return data;
    }

    public static void CreateEmptyData()
    {
        _containerController = new ContainerController(10, _multiSceneSettings.MultiObjectsDataCapacity);
        _objectStateLoader = new ObjectStateLoader(_containerController, _saveAdapter, _loadAdapter);
        _multiObjectLoader = new MultiObjectLoader(_containerController, _multiSceneSettings.NextGlobalIndex);
        Container container = _containerController.CreateContainer(GLOBAL_MULTI_SCENE_NAME);
        container.Loaded();
        LoadMultiSceneQueue();
    }

    public static void LoadData(MOData data)
    {
        if (data == null || !_initialized)
            return;

        _savedStateData = data.SavedStateData;
        _saveAdapter.SetSaveObjectStateAdapter(_savedStateData);
        _loadAdapter.SetLoadObjectStateAdapter(_savedStateData);
        if (_initialized)
        {
            _containerController.LoadContainers(data.ContainerDatas);
            _multiObjectLoader.LoadState(data.GlobalIndex);
        }
        else
        {
            _containerController = new ContainerController(data.ContainerDatas, _multiSceneSettings.MultiObjectsDataCapacity);
            _objectStateLoader = new ObjectStateLoader(_containerController, _saveAdapter, _loadAdapter);
            _multiObjectLoader = new MultiObjectLoader(_containerController, _multiSceneSettings.NextGlobalIndex);
        }

        LoadMultiSceneQueue();
        Debug.Log("[MultiObjectManager]: Loaded into memory <" + _containerController.ContainerDatas.Count + "> scene containers.");
    }

    private static void LoadSettings()
    {
        if (_multiSceneSettings == null)
        {
            _multiSceneSettings = Resources.Load<MultiSceneSettings>(NAME_FILE_SETTINGS);
            if (_multiSceneSettings == null)
                Debug.LogError("[MultiObjectManager]: Not found settings!");
        }
    }

    private static void LoadMultiSceneQueue()
    {
        for (int i = 0; i < _needLoadMultiSceneQueue.Count; i++)
            LoadMultiScene(_needLoadMultiSceneQueue[i]);

        _needLoadMultiSceneQueue.Clear();
    }

    public static void Dispose()
    {
        if (_initialized == false)
            return;

        MultiObjectLoader.ClearAllSpawnedMultiObjects();
        _loadedMultiScene.Clear();
    }

    [Serializable]
    public class MOData
    {
        public int GlobalIndex = 0;
        public Dictionary<int, Container> ContainerDatas;
        public Dictionary<string, object> SavedStateData;
    }
}