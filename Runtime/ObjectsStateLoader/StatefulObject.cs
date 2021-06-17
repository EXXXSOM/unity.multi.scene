using UnityEngine;

public abstract class StatefulObject: MonoBehaviour, IHaveState, IObservableState
{
    [HideInInspector] public int Index;
    private IStatesObserver observer;

    [SerializeField] private int stateValue;

    public int State {
        get => stateValue;
        set
        {
            stateValue = value;
            OnSetState(value);

            NotifyObserver();
        }
    }

    public void SetObserver(int setIndex, IStatesObserver observer)
    {
        Index = setIndex;
        this.observer = observer;
    }

    public void NotifyObserver()
    {
        observer.UpdateState(Index, State);
    }

    public virtual void OnSetState(int value) { }
}
