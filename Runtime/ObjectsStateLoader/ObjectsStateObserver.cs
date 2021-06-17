using System.Collections.Generic;
using UnityEngine;

public class ObjectsStateObserver : MonoBehaviour, IStatesObserver
{
    public List<StatefulObject> AllObjects;
    [HideInInspector] public MultiObjectContainer MultiObjectContainer;

    private void Awake()
    {
        for (int i = 0; i < AllObjects.Count; i++)
        {
            AllObjects[i].SetObserver(i, this);
        }

        ObjectStateLoader.Instance.LoadStates(this);
    }

    private void Start()
    {
    }

    public void UpdateState(int index, int stateValue)
    {
        foreach (var objectStateData in MultiObjectContainer.SavedObjectStates)
        {
            if (objectStateData.GetIndex == index)
            {
                objectStateData.StateValue = stateValue;
                return;
            }
        }

        ObjectStateData newObjectStateData = new ObjectStateData(index, stateValue);
        MultiObjectContainer.SavedObjectStates.Add(newObjectStateData);
    }
}
