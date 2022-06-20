using UnityEngine;

[System.Serializable]
public struct MultiObjectReference
{
    [SerializeField] private int _index;
    [SerializeField] private int _sceneHash;

    public int Index { get => _index; set => _index = value; }
    public int SceneHash => _sceneHash;

    public MultiObjectReference(int index, int sceneHash)
    {
        _index = index;
        _sceneHash = sceneHash;
    }
}
