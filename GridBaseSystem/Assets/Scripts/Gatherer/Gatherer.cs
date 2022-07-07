using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Gatherer : MonoBehaviour
{
    public GathererMining gathererMining;

    private StateMachine _stateMachine;

    public ResourceNode TargetResourceNode { get; set; }
    public Storage TargetStorage { get; set; }



    private void Awake()
    {
        _stateMachine = new StateMachine();
        gathererMining = GetComponent<GathererMining>();

        var empty = new WorkerStates.EmptyWorker(this);
        var moveToSelected = new WorkerStates.MoveToSelectedResource(this);
        var harvest = new WorkerStates.HarvestResource(this);
        var returnToStockpile = new WorkerStates.ReturnToStockpile(this);
        var placeResourcesInStockpile = new WorkerStates.PlaceResourcesInStockpile(this);

        At(empty, moveToSelected, HasTargetResourceNode());
        At(moveToSelected, harvest, ReachedResource());
        At(moveToSelected, empty, RemovedAssignment());
        
        At(harvest, returnToStockpile, RemovedAssignment());
        At(harvest, returnToStockpile, InventoryFull());
        At(harvest, returnToStockpile, ResourceDone());

        At(returnToStockpile, placeResourcesInStockpile, ReachedStockpile());
        At(placeResourcesInStockpile, empty, () => gathererMining.GetHoldingListCount() == 0);

        _stateMachine.SetState(empty);

        void At(IState to, IState from, Func<bool> condition) => _stateMachine.AddTransition(to, from, condition);

        Func<bool> HasTargetResourceNode() => () => TargetResourceNode != null && !TargetResourceNode.IsDepleted;

        Func<bool> RemovedAssignment() => () => TargetResourceNode == null && !InventoryFull().Invoke();
        Func<bool> ReachedResource() => () => BuildingSystem.Instance.GetGrid().GetGridObject(transform.position).GetPlacedObject() is ResourceNode;

        Func<bool> ResourceDone() => () => TargetResourceNode != null && TargetResourceNode.IsDepleted;
        Func<bool> InventoryFull() => () => gathererMining.GetHoldingListCount() >= gathererMining._maxCarried;


        Func<bool> ReachedStockpile() => () => TargetStorage != null &&
        BuildingSystem.Instance.GetGrid().GetGridObject(transform.position).GetPlacedObject() is Storage;

    }



    private void Update() => _stateMachine.Tick();
    public StateMachine GetStateMachine()
    {
        return _stateMachine;
    }
}
