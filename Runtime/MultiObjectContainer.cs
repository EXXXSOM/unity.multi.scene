using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MultiObjectContainer", menuName = "MultiObject/MultiObjectContainer", order = 1)]
public class MultiObjectContainer : ScriptableObject
{
    [Header("Dynamically spawned objects")]
    public List<MultiObjectData> SavedMultiObjects;

    [Header("Saved object states")]
    public List<ObjectStateData> SavedObjectStates;
}