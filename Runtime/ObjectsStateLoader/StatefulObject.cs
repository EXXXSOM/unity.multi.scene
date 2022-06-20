using UnityEngine;

public abstract class StatefulObject : MonoBehaviour
{
    private bool _changes = false;
    public int Index { get; private set; }

    public bool Save(SaveObjectStateAdapter saveAdapter)
    {
        _changes = false;
        OnSave(saveAdapter);
        return _changes;
    }

    protected virtual void OnSave(SaveObjectStateAdapter saveAdapter) { }
    public virtual void Load(LoadObjectStateAdapter loadAdapter) { }
    public virtual void AfterLoad() { }

    protected void NewChanges()
    {
        _changes = true;
    }

    public void SetIndex(int index)
    {
        Index = index;
    }
}
