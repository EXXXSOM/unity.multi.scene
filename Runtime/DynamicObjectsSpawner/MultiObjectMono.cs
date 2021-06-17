using UnityEngine;

public class MultiObjectMono : MonoBehaviour
{
    private ObjectStateLoader objectStateLoader;
    private MultiObjectLoader multiObjectLoader;

    [Header("Testing")]
    public TestMechanic testMechanic;

    private void Awake()
    {
        MultiObjectManager.Bootstrap();

        objectStateLoader = new ObjectStateLoader();
        multiObjectLoader = new MultiObjectLoader();

        multiObjectLoader.RespawnAllItemsOnScene(gameObject.scene.name);
    }

    private void Start()
    {
        testMechanic.State = 2;
    }
}
