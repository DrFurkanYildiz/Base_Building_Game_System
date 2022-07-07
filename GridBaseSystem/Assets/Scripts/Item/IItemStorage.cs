using System;
public interface IItemStorage
{
    // Depolama miktarı değişti..
    event EventHandler OnItemStorageCountChanged;

    int GetItemStoredCount(ItemSO filterItemSO);
    bool TryGetStoredItem(ItemSO[] filterItemSO, out ItemSO itemSO);
    bool TryStoreItem(ItemSO itemSO, int amount = 1);
    ItemSO[] GetItemSOThatCanStore();
}
