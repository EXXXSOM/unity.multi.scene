using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MultiObjectManager
{
    private static Dictionary<string, MultiObjectContainer> allFinedMultiObjectContainers; //Все найденные контейнеры 
    private static Dictionary<string, MultiObjectContainer> usingMultiObjectContainers = new Dictionary<string, MultiObjectContainer>(); //Используеммые контейнеры

    private static MultiObjectContainer currentSceneMultiObjectContainer;
    public static MultiObjectContainer GetCurrentSceneMultiObjectContainer { get { return currentSceneMultiObjectContainer; } }

    public static void Bootstrap()
    {
        SceneManager.activeSceneChanged += SetActiveSceneMultiObjectContainer;
        SceneManager.sceneLoaded += SceneLoaded;
        SceneManager.sceneUnloaded += SceneUnload;

        LoadAllContainers();
    }

    public static void Dispose()
    {
        SceneManager.activeSceneChanged -= SetActiveSceneMultiObjectContainer;
        SceneManager.sceneLoaded -= SceneLoaded;
        SceneManager.sceneUnloaded -= SceneUnload;
    }

    public static MultiObjectContainer GetMultiObjectContainerBySceneName(string sceneName)
    {
        foreach (var container in usingMultiObjectContainers.Values)
        {
            if (container.name == sceneName)
            {
                return container;
            }
        }

        Debug.LogWarning("[MultiObjectManager] Контейнера с именем <" + sceneName + "> не существует!");
        return null;
    }

    private static void SetActiveSceneMultiObjectContainer(Scene oldScene, Scene newScene)
    {
        if (allFinedMultiObjectContainers.ContainsKey(newScene.name))
        {
            Debug.Log("[MultiObjectManager]: Установленая новая активная сцена <" + newScene.name + ">");
            currentSceneMultiObjectContainer = allFinedMultiObjectContainers[newScene.name];
        }
    }

    //Загрузить все существующие контейнеры
    private static void LoadAllContainers()
    {
        allFinedMultiObjectContainers = Resources.LoadAll<MultiObjectContainer>("MultiObjectContainers").ToDictionary(obj => obj.name);

        if (allFinedMultiObjectContainers == null)
            allFinedMultiObjectContainers = new Dictionary<string, MultiObjectContainer>();

        Debug.Log("[MultiObjectManager]: В память загружено <" + allFinedMultiObjectContainers.Count + "> контейнеров.");

        string activeSceneName = SceneManager.GetActiveScene().name;
        if (allFinedMultiObjectContainers.ContainsKey(activeSceneName))
        {
            usingMultiObjectContainers.Add(activeSceneName, allFinedMultiObjectContainers[activeSceneName]);
            Debug.Log("[MultiObjectManager]: В используемые контейнеры загружен <" + activeSceneName + ">.");
        }
    }

    private static void SceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (allFinedMultiObjectContainers.ContainsKey(scene.name))
        {
            if (usingMultiObjectContainers.ContainsKey(scene.name) == false)
            {
                usingMultiObjectContainers.Add(scene.name, allFinedMultiObjectContainers[scene.name]);
                Debug.Log("[MultiObjectManager]: В используемые контейнеры загружен <" + scene.name + ">.");
            }

        }
    }

    private static void SceneUnload(Scene scene)
    {
        if (usingMultiObjectContainers.ContainsKey(scene.name))
        {
            usingMultiObjectContainers.Remove(scene.name);
            Debug.Log("[MultiObjectManager]: Из используемых контейнеров выгружен <" + scene.name + ">.");
        }
    }

}
