public class GameObjectState : StatefulObject
{
    public override void Load(LoadObjectStateAdapter loadAdapter)
    {
        gameObject.SetActive(loadAdapter.GetBool("active"));
    }

    protected override void OnSave(SaveObjectStateAdapter saveAdapter)
    {
        saveAdapter.AddObject("active", gameObject.activeSelf);
        NewChanges();
    }
}
