using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

public class MultiObject : MonoBehaviour
{
    [SerializeField] private MultiObjectReference _multiObjectReference;
    public AsyncOperationHandle<GameObject> AsyncOperationHandle;

    public MultiObjectReference MultiObjectReference => _multiObjectReference;

    public void SetupMetadata(MultiObjectReference multiObjectReference)
    {
        _multiObjectReference = multiObjectReference;
    }
}
