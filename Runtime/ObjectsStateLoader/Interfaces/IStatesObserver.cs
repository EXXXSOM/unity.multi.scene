using System.Collections.Generic;

public interface IStatesObserver
{
    List<StatefulObject> GetAllObjectsWitchState { get; }
    void UpdateState(int index, int state);
}