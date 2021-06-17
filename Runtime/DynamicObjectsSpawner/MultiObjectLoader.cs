using UnityEngine;
using UnityEditor;
using System;

//1. MultiObject - может быть только префаб!;
//2. MultiObjectContainer - все контейнеры должны иметь название сцены;
//3. Контейнеры должны храниться по данному пути "Assets/Resources/MultiObjectContainers".

public class MultiObjectLoader : IDisposable
{
    public static MultiObjectLoader Instance;

    public MultiObjectLoader()
    {
        Instance = this;
    }

    public void Dispose()
    {
        Instance = null;
    }

    public void Spawn(GameObject multiObjectPrefab, string sceneName)
    {
        if (PrefabUtility.GetPrefabAssetType(multiObjectPrefab) == PrefabAssetType.NotAPrefab)
        {
            Debug.LogWarning("[MultiObjectLoader] Объект <" + multiObjectPrefab.name + "> не префаб!");
            return;
        }

        MultiObjectContainer containerForScene =  MultiObjectManager.GetMultiObjectContainerBySceneName(sceneName);

        if (containerForScene != null)
        {
            MultiObjectData multiObject = new MultiObjectData(multiObjectPrefab.transform.position, multiObjectPrefab, containerForScene.SavedMultiObjects.Count);
            containerForScene.SavedMultiObjects.Add(multiObject);
        }
    }

    public void Despawn(GameObject gameObject, string sceneName)
    {
        MultiObjectContainer containerForScene = MultiObjectManager.GetMultiObjectContainerBySceneName(sceneName);

        if (containerForScene == null)
        {
            Debug.LogWarning("[MultiObjectLoader] Объект с именем <" + gameObject.name + "> не может быть деспавнен, он не существует в контейнере!");
            return;
        }
        else
        {
            MultiObjectData multiObject = RemoveMultiObjectFromContainer(containerForScene, gameObject);
            if (multiObject != null)
            {
                GameObject.Destroy(multiObject.GameObject);
            }
        }
    }

    //Загрузить мульти объекты сцены
    public void RespawnAllItemsOnScene(string sceneName)
    {
        MultiObjectContainer containerForScene = MultiObjectManager.GetMultiObjectContainerBySceneName(sceneName);
        if (containerForScene != null)
        {
            SpawnAllItemsFromContainer(containerForScene);
        }
    }

    private void SpawnAllItemsFromContainer(MultiObjectContainer container)
    {
        foreach (var multiObject in container.SavedMultiObjects)
        {
            GameObjectSpawner.SpawnLocalGameObject(multiObject.GameObject, multiObject.Position);
        }
    }

    private MultiObjectData RemoveMultiObjectFromContainer(MultiObjectContainer container, GameObject gameObject)
    {
        for (int i = 0; i < container.SavedMultiObjects.Count; i++)
        {
            if (container.SavedMultiObjects[i].GameObject == gameObject)
            {
                MultiObjectData multiObjectReference = container.SavedMultiObjects[i];
                container.SavedMultiObjects.RemoveAt(i);

                return multiObjectReference;
            }
        }

        Debug.LogWarning("[MultiObjectManager] В контейнере объекта с именем <" + gameObject.name + "> не существует!");
        return null;
    }
}