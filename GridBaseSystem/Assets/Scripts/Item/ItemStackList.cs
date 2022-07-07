using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemStackList
{

    private List<ItemStack> itemStackList;

    public ItemStackList()
    {
        itemStackList = new List<ItemStack>();
    }

    public List<ItemStack> GetItemStackList()
    {
        return itemStackList;
    }
    //İtemi yığına ekle.
    public void AddItemToItemStack(ItemSO itemSO, int amount = 1)
    {
        ItemStack itemStack = GetItemStackWithItemType(itemSO);
        if (itemStack != null)
        {
            itemStack.amount += amount;
        }
        else
        {
            itemStack = new ItemStack { itemSO = itemSO, amount = amount };
            itemStackList.Add(itemStack);
        }
    }
    // Depolanan Eşya Sayısını Al
    public int GetItemStoredCount(ItemSO filterItemSO)
    {
        int amount = 0;
        foreach (ItemStack itemStack in itemStackList)
        {
            if (filterItemSO == GameAssets.i.itemSO_Refs.any || filterItemSO == itemStack.itemSO)
            {
                amount += itemStack.amount;
            }
        }
        return amount;
    }

    //Item yığınına item eklenebilir mi?
    public bool CanAddItemToItemStack(ItemSO itemSO, int amount = 1)
    {
        ItemStack itemStack = GetItemStackWithItemType(itemSO);
        if (itemStack != null)
        {
            // Yığın zaten var, yer var mı?
            if (itemStack.amount + amount <= itemSO.maxStackAmount)
            {
                // Eklenebilir
                return true;
            }
            else
            {
                // Yığın dolu
                return false;
            }
        }
        else
        { 
            // Öğe yığını yok, ekleyebilir
            return true;
        }
    }

    //Item yığınını bul.
    public ItemStack GetItemStackWithItemType(ItemSO itemSO)
    {
        foreach (ItemStack itemStack in itemStackList)
        {
            if (itemStack.itemSO == itemSO)
            {
                return itemStack;
            }
        }
        return null;
    }
 
    //Filtreli İlk Öğe Yığınını Alın
    public ItemStack GetFirstItemStackWithFilter(ItemSO[] filterItemSO)
    {
        foreach (ItemSO itemSO in filterItemSO)
        {
            foreach (ItemStack itemStack in itemStackList)
            {
                if (itemStack.itemSO == itemSO)
                {
                    return itemStack;
                }
            }
        }
        return null;
    }
}
