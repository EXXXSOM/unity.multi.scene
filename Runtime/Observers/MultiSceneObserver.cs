using System.Collections.Generic;
using UnityEngine;

public class MultiSceneObserver : MonoBehaviour, IStatesObserver
{
    [Header("Динамические объекты сцены")]
    [SerializeField] private bool _loadMultiObjects = true;
    [SerializeField] public bool IsLoadMultiObjects => _loadMultiObjects;
    [SerializeField] private List<MultiObject> staticMultiObjectsScene = new List<MultiObject>();
    public List<MultiObject> GetAllStaticMultiObject => staticMultiObjectsScene;

    [Header("Объекты с состоянием")]
    [SerializeField] private bool _loadStates = true;
    [SerializeField] public bool IsloadStates => _loadStates;
    [SerializeField] private List<StatefulObject> _allObjectsWitchState = new List<StatefulObject>();
    public List<StatefulObject> GetAllObjectsWitchState => _allObjectsWitchState;

    public List<MultiObject> SpawnedMultiObjects = new List<MultiObject>(10);

    [HideInInspector] public CacheMultiObjectContainer MultiObjectContainer;

    private void Awake()
    {
        for (int i = 0; i < _allObjectsWitchState.Count; i++)
        {
            _allObjectsWitchState[i].SetObserver(i, this);
        }

        MultiObjectManager.LoadMultiScene(this);
    }

    public void UpdateState(int index, int stateValue)
    {
        MultiObjectContainer.AddObjectState(index, stateValue);
    }

    //For EDITOR
    public void SetMultiSceneObserver(List<MultiObject> listMultiObjects, List<StatefulObject> objectsWitchState)
    {
        _allObjectsWitchState = objectsWitchState;
        staticMultiObjectsScene = listMultiObjects;
    }
}
