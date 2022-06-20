using System.Collections.Generic;
using UnityEngine;

public class MultiScene : MonoBehaviour
{
    public BakedMultiObjectData BakedData;

#if UNITY_EDITOR
    [Header("Динамические объекты сцены")]
    [SerializeField] private List<MultiObject> _staticMultiObjectsSceneForBacking;
#endif

    [Header("Объекты с состоянием")]
    [SerializeField] private List<StatefulObject> _allObjectsWitchState;

    public string SceneName => gameObject.scene.name;
    public List<StatefulObject> GetAllObjectsWitchState => _allObjectsWitchState;
#if UNITY_EDITOR
    public List<MultiObject> GetAllStaticMultiObject => _staticMultiObjectsSceneForBacking;
#endif

    private void Awake()
    {
        if (MultiSceneManager.AutoLoadMultiScene == false)
            return;

        Debug.Log("Start load multi scene " + gameObject.scene.name + " " + gameObject.scene.name.GetHashCode());

        if (_allObjectsWitchState != null)
        {
            for (int i = 0; i < _allObjectsWitchState.Count; i++)
                _allObjectsWitchState[i].SetIndex(i);
        }

        MultiSceneManager.LoadMultiScene(this);
    }

    //For EDITOR
#if UNITY_EDITOR
    public void SetMultiSceneObserver(List<MultiObject> listMultiObjects, List<StatefulObject> objectsWitchState)
    {
        _allObjectsWitchState = objectsWitchState;
        _staticMultiObjectsSceneForBacking = listMultiObjects;
    }
#endif
}
