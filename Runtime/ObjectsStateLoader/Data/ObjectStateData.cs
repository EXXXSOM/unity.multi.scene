using UnityEngine;

[System.Serializable]
public class ObjectStateData
{
    [SerializeField] private int index;
    [SerializeField] private int stateValue;

    public int GetIndex { get => index; }
    public int StateValue { get => stateValue; set => stateValue = value; }

    public ObjectStateData(int index, int stateValue)
    {
        this.index = index;
        this.stateValue = stateValue;
    }
}
