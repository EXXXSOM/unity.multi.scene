public interface IObservableState
{
    void SetObserver(int setIndex, IStatesObserver observer);
    void NotifyObserver();
}