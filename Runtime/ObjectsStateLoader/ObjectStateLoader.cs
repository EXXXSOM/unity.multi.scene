using System;

public class ObjectStateLoader : IDisposable
{
    public static ObjectStateLoader Instance;

    public ObjectStateLoader()
    {
        Instance = this;
    }

    public void Dispose()
    {
        Instance = null;
    }

    public void LoadStates(ObjectsStateObserver observer)
    {
        MultiObjectContainer multiObjectContainer = MultiObjectManager.GetMultiObjectContainerBySceneName(observer.gameObject.scene.name);
        observer.MultiObjectContainer = multiObjectContainer;

        foreach (var savedObjectState in multiObjectContainer.SavedObjectStates)
        {
            observer.AllObjects[savedObjectState.GetIndex].State = savedObjectState.StateValue;
        }
    }

    public void ResetStateOfObjectsOnScene(string sceneName)
    {
        MultiObjectContainer multiObjectContainer = MultiObjectManager.GetMultiObjectContainerBySceneName(sceneName);
        multiObjectContainer.SavedObjectStates.Clear();
    }
}
