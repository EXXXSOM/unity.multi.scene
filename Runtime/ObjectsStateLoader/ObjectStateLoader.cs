using System.Collections.Generic;

public class ObjectStateLoader
{
    private readonly ContainerController _containerController;
    private readonly SaveObjectStateAdapter _saveAdapter;
    private readonly LoadObjectStateAdapter _loadAdapter;

    public ObjectStateLoader(ContainerController containerController,
        SaveObjectStateAdapter saveAdapter,
        LoadObjectStateAdapter loadAdapter)
    {
        _containerController = containerController;
        _saveAdapter = saveAdapter;
        _loadAdapter = loadAdapter;
    }

    public void LoadStates(MultiScene multiScene, Container container)
    {
        if (multiScene.GetAllObjectsWitchState != null && container.SavedIndexStateObjects != null)
        {
            for (int i = 0; i < container.SavedIndexStateObjects.Length; i++)
            {
                if (container.SavedIndexStateObjects[i] == false)
                {
                    multiScene.GetAllObjectsWitchState[i].AfterLoad();
                    continue;
                }

                _loadAdapter.SetKeyOffset(multiScene, i);
                multiScene.GetAllObjectsWitchState[i].Load(_loadAdapter);
                multiScene.GetAllObjectsWitchState[i].AfterLoad();
            }

            _loadAdapter.ResetAdapter();
        }
        else
        {
            for (int i = 0; i < multiScene.GetAllObjectsWitchState.Count; i++)
            {
                multiScene.GetAllObjectsWitchState[i].AfterLoad();
            }
        }
    }

    public void SaveAllStates(List<MultiScene> multiScenes)
    {
        Container container = null;
        for (int i = 0; i < multiScenes.Count; i++)
        {
            if (_containerController.GetContainer(multiScenes[i].SceneName, out container))
                SaveStateObjects(multiScenes[i], container);
        }
    }

    public void SaveStateObjects(MultiScene multiScene, Container container)
    {
        if (multiScene.GetAllObjectsWitchState != null)
        {
            bool[] savedIndexes;

            if (container.SavedIndexStateObjects == null)
            {
                savedIndexes = new bool[multiScene.GetAllObjectsWitchState.Count];
                container.SetSavedIndexStateObjects(savedIndexes);
            }
            else
            {
                savedIndexes = container.SavedIndexStateObjects;
            }

            for (int i = 0; i < multiScene.GetAllObjectsWitchState.Count; i++)
            {
                _saveAdapter.SetKeyOffset(multiScene, i);
                savedIndexes[i] = multiScene.GetAllObjectsWitchState[i].Save(_saveAdapter);
            }
        
            _saveAdapter.ResetAdapter();
        }
    }
}
