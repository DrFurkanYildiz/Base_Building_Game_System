using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class SearchForStorage : IState
{
    private readonly Technician technician;
    public SearchForStorage(Technician technician)
    {
        this.technician = technician;
    }

    public void OnEnter()
    {
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
        foreach (var item in BuildingSystem.Instance.placedBuildingList)
        {
            if (item is Storage)
                technician.TargetStorage = item as Storage;
        }
    }

}
