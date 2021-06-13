using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System;

//1. MultiObject - может быть только префаб!;
//2. MultiObjectContainer - все контейнеры должны иметь название сцены;
//3. Контейнеры должны храниться по данному пути "Assets/Resources/MultiObjectContainers".

public class MultiObjectManager : IDisposable
{
    public MultiObjectManager Instance;

    private MultiObjectContainer[] allFinedMultiObjectContainers;  //Все найденные контейнеры 
    private List<MultiObjectContainer> usingMultiObjectContainers = new List<MultiObjectContainer>(); //Используеммые контейнеры

    public MultiObjectManager()
    {
        Instance = this;

        LoadAllContainers();

        SceneManager.sceneLoaded += SceneLoaded;
        SceneManager.sceneUnloaded += SceneUnload;
    }

    public void Dispose()
    {
        Instance = null;

        SceneManager.sceneLoaded -= SceneLoaded;
        SceneManager.sceneUnloaded -= SceneUnload;
    }

    private void SceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        foreach (var container in allFinedMultiObjectContainers)
        {
            if (container.name == scene.name)
            {
                usingMultiObjectContainers.Add(container);
                break;
            }
        }
    }

    private void SceneUnload(Scene scene)
    {
        foreach (var container in allFinedMultiObjectContainers)
        {
            if (container.name == scene.name)
            {
                usingMultiObjectContainers.Remove(container);
                break;
            }
        }
    }

    public void Spawn(GameObject multiObjectPrefab, string sceneName)
    {
        if (PrefabUtility.GetPrefabAssetType(multiObjectPrefab) == PrefabAssetType.NotAPrefab)
        {
            Debug.LogWarning("[MultiObjectManager] Объект <" + multiObjectPrefab.name + "> не префаб!");
            return;
        }

        MultiObjectContainer containerForScene = null;

        foreach (var container in usingMultiObjectContainers)
        {
            if (container.name == sceneName)
            {
                containerForScene = container;
                break;
            }
        }

        if (containerForScene == null)
        {
            Debug.LogWarning("[MultiObjectManager] Контейнера с именем <" + sceneName + "> не существует!");
            return;
        }
        else
        {
            MultiObject multiObject = new MultiObject(multiObjectPrefab.transform.position, multiObjectPrefab, containerForScene.SavedMultiObjects.Count);
            containerForScene.SavedMultiObjects.Add(multiObject);
        }
    }

    public void Despawn(GameObject gameObject, string sceneName)
    {
        MultiObjectContainer containerForScene = null;

        foreach (var container in usingMultiObjectContainers)
        {
            if (container.name == sceneName)
            {
                containerForScene = container;
                break;
            }
        }

        if (containerForScene == null)
        {
            Debug.LogWarning("[MultiObjectManager] Объект с именем <" + gameObject.name + "> не может быть деспавнен, он не существует в контейнере!");
            return;
        }
        else
        {
            MultiObject multiObject = RemoveMultiObjectFromContainer(containerForScene, gameObject);
            if (multiObject != null)
            {
                GameObject.Destroy(multiObject.GameObject);
            }
        }
    }

    //Загрузить мульти объекты сцены
    public void RespawnAllItemsOnScene(string sceneName)
    {
        foreach (var container in allFinedMultiObjectContainers)
        {
            if (container.name == sceneName)
            {
                SpawnAllItemsFromContainer(container);
                return;
            }
        }

        Debug.LogWarning("[MultiObjectManager] Контейнера с именем <" + sceneName + "> не существует!");
    }

    //Загрузить все существующие контейнеры
    private void LoadAllContainers()
    {
        allFinedMultiObjectContainers = Resources.LoadAll<MultiObjectContainer>("MultiObjectContainers");
    }

    private void SpawnAllItemsFromContainer(MultiObjectContainer container)
    {
        foreach (var multiObject in container.SavedMultiObjects)
        {
            GameObjectSpawner.SpawnLocalGameObject(multiObject.GameObject, multiObject.Position);
        }
    }

    private MultiObject RemoveMultiObjectFromContainer(MultiObjectContainer container, GameObject gameObject)
    {
        for (int i = 0; i < container.SavedMultiObjects.Count; i++)
        {
            if (container.SavedMultiObjects[i].GameObject == gameObject)
            {
                MultiObject multiObjectReference = container.SavedMultiObjects[i];
                container.SavedMultiObjects.RemoveAt(i);

                return multiObjectReference;
            }
        }

        Debug.LogWarning("[MultiObjectManager] В контейнере объекта с именем <" + gameObject.name + "> не существует!");
        return null;
    }
}