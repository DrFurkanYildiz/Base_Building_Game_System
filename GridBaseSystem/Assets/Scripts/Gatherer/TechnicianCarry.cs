using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechnicianCarry : MonoBehaviour, IItemStorage
{
    public event EventHandler OnItemStorageCountChanged;
    private Technician technician;

    public ItemSO miningResourceItem;
    public ItemSO grabFilterItemSO;
    public List<WorldItem> holdingItemList = new List<WorldItem>();

    public int _maxCarried = 3;
    private float miningTimer;
    private int storedItemCount;
    public Vector2Int technicianGridPosition;
    

    private void Awake()
    {
        technician = GetComponent<Technician>();
        grabFilterItemSO = GameAssets.i.itemSO_Refs.any;
    }

    private void Update()
    {
        technicianGridPosition = BuildingSystem.Instance.GetGridPosition(transform.position);

        if (miningResourceItem == null)
        {
            // No resources in range!
            return;
        }


        miningTimer -= Time.deltaTime;
        if (miningTimer <= 0f)
        {
            miningTimer += miningResourceItem.miningTimer;

            storedItemCount += 1;
            OnItemStorageCountChanged?.Invoke(this, EventArgs.Empty);
            technician.TargetStorage.TriggerGridObjectChanged();
        }
    }
    public int GetHoldingListCount()
    {
        return holdingItemList.Count;
    }
    public ItemSO GetMiningResourceItem()
    {
        return miningResourceItem;
    }
    public int GetItemStoredCount(ItemSO filterItemScriptableObject)
    {
        return storedItemCount;
    }

    public bool TryGetStoredItem(ItemSO[] filterItemSO, out ItemSO itemSO)
    {
        if (ItemSO.IsItemSOInFilter(GameAssets.i.itemSO_Refs.none, filterItemSO) ||
            ItemSO.IsItemSOInFilter(miningResourceItem, filterItemSO))
        {
            // If filter matches any or filter matches this itemType
            if (storedItemCount > 0)
            {
                storedItemCount--;
                itemSO = miningResourceItem;
                OnItemStorageCountChanged?.Invoke(this, EventArgs.Empty);
                technician.TargetStorage?.TriggerGridObjectChanged();
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

    public ItemSO[] GetItemSOThatCanStore()
    {
        return new ItemSO[] { GameAssets.i.itemSO_Refs.none };
    }

    public bool TryStoreItem(ItemSO itemScriptableObject)
    {
        return false;
    }

    public bool TryStoreItem(ItemSO itemSO, int amount = 1)
    {
        throw new NotImplementedException();
    }
}
