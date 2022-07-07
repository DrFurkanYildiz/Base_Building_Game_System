using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Storage : PlacedObject, IItemStorage
{
    public event EventHandler OnItemStorageCountChanged;

    private ItemStackList itemStackList;

    [SerializeField]private int amountC;
    [SerializeField]private int amountE;

    private void Update()
    {
        if (itemStackList != null)
        {
            amountC = itemStackList.GetItemStoredCount(GameAssets.i.itemSO_Refs.carbohydrate);
            amountE = itemStackList.GetItemStoredCount(GameAssets.i.itemSO_Refs.energy);
        }
    }
    protected override void Setup()
    {
        //Debug.Log("Storage.Setup()");
        itemStackList = new ItemStackList();
        BuildingSystem.Instance.RoadChangeVisual();
    }

    public ItemStackList GetItemStackList()
    {
        return itemStackList;
    }
    public int GetItemStoredCount(ItemSO filterItemSO)
    {
        return itemStackList.GetItemStoredCount(filterItemSO);
    }


    public bool TryGetStoredItem(ItemSO[] filterItemSO, out ItemSO itemSO)
    {
        // Filtre herhangi biriyle eşleşirse veya filtre bu itemType ile eşleşirse..........????????
        ItemStack itemStack = itemStackList.GetFirstItemStackWithFilter(filterItemSO);
        if (itemStack != null && itemStack.amount > 0)
        {
            itemStack.amount -= 1;
            itemSO = itemStack.itemSO;
            OnItemStorageCountChanged?.Invoke(this, EventArgs.Empty);
            TriggerGridObjectChanged();
            return true;
        }
        else
        {
            itemSO = null;
            return false;
        }
    }

    public bool TryStoreItem(ItemSO itemSO, int amount = 1)
    {
        if (itemStackList.CanAddItemToItemStack(itemSO, amount))
        {
            itemStackList.AddItemToItemStack(itemSO, amount);
            OnItemStorageCountChanged?.Invoke(this, EventArgs.Empty);
            TriggerGridObjectChanged();
            return true;
        }
        else
        {
            return false;
        }
    }

    public ItemSO[] GetItemSOThatCanStore()
    {
        return new ItemSO[] { GameAssets.i.itemSO_Refs.any };
    }

}
