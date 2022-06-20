public interface IHaveState
{
    int State { get; set; }
    void LoadState(int value);
}