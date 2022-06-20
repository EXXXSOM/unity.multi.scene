using UnityEngine;

public class MultiSceneSettings : ScriptableObject
{
    public int NextGlobalIndex = 0;
    public string PathFolderWithBakedMultiObjects = "BakedMultiObjectData";
    public int MultiObjectsDataCapacity = 10;
    public bool AutoLoadMultiScene = true;
}
