using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
#endif

public class ContainerController
{
    public Dictionary<int, Container> ContainerDatas => _containerDatas; //SaveData
    private Dictionary<int, Container> _containerDatas;
    private int _multiObjectDataCapacity = 15;

    public ContainerController(int capacityContainers, int multiObjectDataCapacity)
    {
        _containerDatas = new Dictionary<int, Container>(capacityContainers);
        _multiObjectDataCapacity = multiObjectDataCapacity;
    }

    public ContainerController(Dictionary<int, Container> containerDatas, int multiObjectDataCapacity)
    {
        _containerDatas = containerDatas;
#if UNITY_EDITOR
        long size = 0;
        using (Stream s = new MemoryStream())
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(s, _containerDatas);
            size = s.Length;
            Debug.Log("[MultiObjectManager] MultiScene cache size: " + size + " byte.");
        }
#endif
    }

    public void LoadContainers(Dictionary<int, Container> containerDatas)
    {
        _containerDatas = containerDatas;
    }

    public Container GetOrCteateContainer(string sceneName)
    {
        int sceneHash = sceneName.GetHashCode();

        Container container = null;
        if (_containerDatas.TryGetValue(sceneHash, out container))
            return _containerDatas[sceneHash];
        else
            return CreateContainer(sceneName);
    }

    public bool GetContainer(string sceneName, out Container container)
    {
        return _containerDatas.TryGetValue(sceneName.GetHashCode(), out container);
    }
    public bool GetContainer(int sceneHash, out Container container)
    {
        return _containerDatas.TryGetValue(sceneHash, out container);
    }

    public bool CheckContainer(string sceneName)
    {
        int sceneHash = sceneName.GetHashCode();
        return _containerDatas.ContainsKey(sceneHash);
    }
    public bool CheckContainer(int sceneHash)
    {
        return _containerDatas.ContainsKey(sceneHash);
    }

    public List<Container> GetContainers()
    {
        return _containerDatas.Values.ToList();
    }

    public Container CreateContainer(string sceneName)
    {
        Debug.Log("[ContainerController]: Created container for new scene <" + sceneName + ">");
        Container container = new Container(sceneName.GetHashCode(), _multiObjectDataCapacity);
        _containerDatas.Add(container.SceneHash, container);
        return container;
    }
}
