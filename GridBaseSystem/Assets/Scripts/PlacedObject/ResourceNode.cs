using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceNode : PlacedObject
{
    public event EventHandler OnItemStorageCountChanged;
    [SerializeField] private ItemSO resourceItem;
    private int maxWorkerAmount = 3;
    public List<Gatherer> workers;

    [SerializeField] private int _totalAvailable = 20;

    public bool IsDepleted => _totalAvailable <= 0;

    private void Update()
    {
        foreach (var item in workers)
        {
            item.TargetResourceNode = this;
        }
    }

    protected override void Setup()
    {
        //Debug.Log("ResourceNode.Setup()");
        BuildingSystem.Instance.RoadChangeVisual();
        workers = new List<Gatherer>();
    }
    public int GetMaxWorkerAmount()
    {
        return maxWorkerAmount;
    }
    public int GetTotalAvailableReseurce()
    {
        return _totalAvailable;
    }
    public ItemSO GetResourceItem()
    {
        return resourceItem;
    }
    public bool TryAvailableResource()
    {
        if (_totalAvailable <= 0)
            return false;

        _totalAvailable--;
        //OnItemStorageCountChanged.Invoke(this, EventArgs.Empty);
        return true;
    }

}
