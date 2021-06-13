using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MultiObjectContainer", menuName = "MultiObject/MultiObjectContainer", order = 1)]
public class MultiObjectContainer : ScriptableObject
{
    public List<MultiObject> SavedMultiObjects;
}