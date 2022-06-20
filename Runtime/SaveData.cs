using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    //MultiScenes
    private MultiObjectManager.MOData _multiObjectManagerData;
    public MultiObjectManager.MOData MultiObjectManagerData => _multiObjectManagerData;

    //InventoryItem
    private List<MultiObjectReference> _multiObjectInInventory;
    public List<MultiObjectReference> MultiObjectInInventory => _multiObjectInInventory;

    public SaveData()
    {
        _multiObjectManagerData = MultiObjectManager.SaveData();
        _multiObjectInInventory = Inventory.Instance.SaveInventory();
    }
}
