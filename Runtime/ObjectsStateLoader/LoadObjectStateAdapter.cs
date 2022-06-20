using System.Collections.Generic;

public class LoadObjectStateAdapter
{
    private Dictionary<string, object> _storage;
    private string _keyOffset = string.Empty;

    public LoadObjectStateAdapter(Dictionary<string, object> savedStateData)
    {
        _storage = savedStateData;
    }

    public void SetLoadObjectStateAdapter(Dictionary<string, object> savedStateData)
    {
        _storage = savedStateData;
    }

    public void SetKeyOffset(MultiScene multiScene, int keyOffset)
    {
        _keyOffset = multiScene.SceneName + keyOffset;
    }

    public T GetValueByType<T>(string name)
    {
        return (T)_storage[_keyOffset + name];
    }

    public bool GetBool(string name)
    {
        return (bool)_storage[_keyOffset + name];
    }

    public int GetInt(string name)
    {
        return (int)_storage[_keyOffset + name];
    }

    public float GetFloat(string name)
    {
        return (float)_storage[_keyOffset + name];
    }

    public string GetString(string name)
    {
        return (string)_storage[_keyOffset + name];
    }

    public void ResetAdapter()
    {
        _keyOffset = string.Empty;
    }
}
