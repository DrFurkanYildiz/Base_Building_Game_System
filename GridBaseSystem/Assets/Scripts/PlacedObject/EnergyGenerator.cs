using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Smelter
/// </summary>
public class EnergyGenerator : PlacedObject, IItemStorage
{
    public event EventHandler OnItemStorageCountChanged;


    private ItemRecipeSO itemRecipeSO;
    private ItemStackList inputItemStackList;
    private ItemStackList outputItemStackList;
    [SerializeField]private float craftingProgress;

    [SerializeField] private int amountC;
    [SerializeField] private int amountE;

    protected override void Setup()
    {
        //Debug.Log("Smelter.Setup()");
        inputItemStackList = new ItemStackList();
        outputItemStackList = new ItemStackList();

        SetItemRecipeScriptableObject(GameAssets.i.itemRecipeSO_Refs.carbohydrateToEnergyRecipe);
        BuildingSystem.Instance.RoadChangeVisual();
    }

    private void Update()
    {

        if (inputItemStackList != null)
        {
            amountC = inputItemStackList.GetItemStoredCount(GameAssets.i.itemSO_Refs.carbohydrate);
        }
        if (outputItemStackList != null)
        {
            amountE = outputItemStackList.GetItemStoredCount(GameAssets.i.itemSO_Refs.energy);
        }



        if (!HasItemRecipe()) return;

        if (HasEnoughItemsToCraft())
        {
            craftingProgress += Time.deltaTime;

            if(craftingProgress >= itemRecipeSO.craftingEffort)
            {
                //Item üretimi tamamlandı.
                craftingProgress = 0f;

                // Craft edilen itemi çıkış item listesine ekle.
                foreach (ItemRecipeSO.RecipeItem recipeItem in itemRecipeSO.outputItemList)
                {
                    outputItemStackList.AddItemToItemStack(recipeItem.item, recipeItem.amount);
                }

                //Item girdilerini Tüket.
                foreach (ItemRecipeSO.RecipeItem recipeItem in itemRecipeSO.inputItemList)
                {
                    ItemStack itemStack = inputItemStackList.GetItemStackWithItemType(recipeItem.item);
                    itemStack.amount -= recipeItem.amount;
                }

                OnItemStorageCountChanged?.Invoke(this, EventArgs.Empty);
                TriggerGridObjectChanged();
            }
        }
    }

    // UI öğesinde kullanılacak.
    public float GetCraftingProgressNormalized()
    {
        if (HasItemRecipe())
        {
            return craftingProgress / itemRecipeSO.craftingEffort;
        }
        else
        {
            return 0f;
        }
    }

    // Depolanan Öğe Sayısını Al
    public int GetItemStoredCount(ItemSO filterItemSO)
    {
        int amount = 0;

        amount += outputItemStackList.GetItemStoredCount(filterItemSO);
        amount += inputItemStackList.GetItemStoredCount(filterItemSO);

        return amount;
    }



    // Depolayabilen Itemi Alın
    public ItemSO[] GetItemSOThatCanStore()
    {
        if (!HasItemRecipe()) return new ItemSO[] { GameAssets.i.itemSO_Refs.none };

        List<ItemSO> canStoreItemSOList = new List<ItemSO>();
        foreach (ItemRecipeSO.RecipeItem recipeItem in itemRecipeSO.inputItemList)
        {
            canStoreItemSOList.Add(recipeItem.item);
        }

        return canStoreItemSOList.ToArray();
    }

    // Filtre herhangi biriyle eşleşirse veya filtre bu itemType ile eşleşirse..........????????
    public bool TryGetStoredItem(ItemSO[] filterItemSO, out ItemSO itemSO)
    {
        if (!HasItemRecipe())
        {
            itemSO = null;
            return false;
        }

        if (ItemSO.IsItemSOInFilter(GameAssets.i.itemSO_Refs.any, filterItemSO) ||
            ItemSO.IsItemSOInFilter(itemRecipeSO.outputItemList[0].item, filterItemSO))
        {
            // Filtre herhangi biriyle eşleşirse veya filtre bu itemType ile eşleşirse..........????????
            ItemStack itemStack = outputItemStackList.GetItemStackWithItemType(itemRecipeSO.outputItemList[0].item);
            if (itemStack != null)
            {
                if (itemStack.amount > 0)
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
            else
            {
                itemSO = null;
                return false;
            }
        }
        else
        {
            itemSO = null;
            return false;
        }
    }

    // Giriş yığınına öğe ekleyebilir mi?
    public bool TryStoreItem(ItemSO itemSO, int amount = 1)
    {
        if (!HasItemRecipe()) return false;

        foreach (ItemRecipeSO.RecipeItem recipeItem in itemRecipeSO.inputItemList)
        {
            if (itemSO == recipeItem.item)
            {
                // Giriş yığınına öğe ekleyebilir mi?
                if (inputItemStackList.CanAddItemToItemStack(itemSO, amount))
                {
                    inputItemStackList.AddItemToItemStack(itemSO, amount);
                    OnItemStorageCountChanged?.Invoke(this, EventArgs.Empty);
                    TriggerGridObjectChanged();
                    return true;
                }
                else
                {
                    // Bu öğe yığına sığmıyor
                    return false;
                }
            }
        }
        return false;
    }



    //Üretim için yeterli öğe var mı kontrol et.
    private bool HasEnoughItemsToCraft()
    {
        if (!HasItemRecipe()) return false;

        foreach (ItemRecipeSO.RecipeItem recipeItem in itemRecipeSO.inputItemList)
        {
            ItemStack itemStack = inputItemStackList.GetItemStackWithItemType(recipeItem.item);
            if (itemStack == null)
            {
                // Bu öğe türünde hiçbir öğe yığını yok
                return false;
            }
            else
            {
                if (itemStack.amount < recipeItem.amount)
                {
                    // Bu öğe türünden yeterli miktarda yok
                    return false;
                }
            }
        }
        // Her şey burada, işlemeye hazır
        return true;
    }

    public bool HasItemRecipe()
    {
        return itemRecipeSO != null;
    }

    public ItemRecipeSO GetItemRecipeSO()
    {
        return itemRecipeSO;
    }
    //UI içerisinden Tarif eklenecek..Yukarda tarif olup olmadığını kontrol ediyor..
    public void SetItemRecipeScriptableObject(ItemRecipeSO itemRecipeSO)
    {
        this.itemRecipeSO = itemRecipeSO;
    }
}
