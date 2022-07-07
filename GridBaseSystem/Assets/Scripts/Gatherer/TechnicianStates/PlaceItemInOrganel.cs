using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class PlaceItemInOrganel : IState
{
    private readonly Technician technician;
    public PlaceItemInOrganel(Technician technician)
    {
        this.technician = technician;
    }

    public void OnEnter()
    {
        //technician.technicianCarry.miningResourceItem = null;
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
        if (technician.TargetOrganel != null)
        {

            if (technician.TargetOrganel is IItemStorage)
            {
                IItemStorage energyGenerator = technician.TargetOrganel as IItemStorage;

                //if(energyGenerator.TryGetStoredItem(energyGenerator.GetItemSOThatCanStore(),out ItemSO itemSO))
                //{
                //    Debug.Log("Eklenebilir");
                //}

                if (energyGenerator.TryStoreItem(technician.technicianCarry.holdingItemList[0].GetItemSO(), technician.technicianCarry.GetHoldingListCount()))
                {
                    for (int i = 0; i < technician.technicianCarry.holdingItemList.Count; i++)
                    {
                        technician.technicianCarry.holdingItemList[i].DestroySelf();
                    }
                    technician.technicianCarry.holdingItemList.Clear();
                }
            }
        }
    }
}
