using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class GetItemToOrganel : IState
{
    private readonly Technician technician;

    public GetItemToOrganel(Technician technician)
    {
        this.technician = technician;
    }


    public void OnEnter()
    {
        //technician.TargetStorage.TryGetStoredItem(technician.TargetStorage.GetItemSOThatCanStore(), out ItemSO item);
        technician.technicianCarry.miningResourceItem = GameAssets.i.itemSO_Refs.carbohydrate;
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
        if (technician.TargetStorage != null)
        {
            ItemSO[] dropFilterItemSO = new ItemSO[] { technician.technicianCarry.miningResourceItem };

            if (technician.TargetStorage.TryGetStoredItem(dropFilterItemSO, out ItemSO itemScriptableObject))
            {
                WorldItem worldItem = WorldItem.Create(technician.technicianCarry.technicianGridPosition, itemScriptableObject);
                technician.technicianCarry.holdingItemList.Add(worldItem);

            }
            else
            {
                Debug.Log("Ürün Yok!");
            }

        }
    }
}
