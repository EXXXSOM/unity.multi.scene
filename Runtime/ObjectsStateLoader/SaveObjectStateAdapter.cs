using System.Collections.Generic;

public class SaveObjectStateAdapter
{
    private Dictionary<string, object> _storage;
    private string _keyOffset = string.Empty;

    public SaveObjectStateAdapter(Dictionary<string, object> storage)
    {
        _storage = storage;
    }

    public void SetSaveObjectStateAdapter(Dictionary<string, object> storage)
    {
        _storage = storage;
    }

    public void SetKeyOffset(MultiScene multiScene, int keyOffset)
    {
        _keyOffset = multiScene.SceneName + keyOffset;
    }

    public void AddObject(string name, object value)
    {
        _storage.Add(_keyOffset + name, value);
    }

    public void ResetAdapter()
    {
        _keyOffset = string.Empty;
    }
}
