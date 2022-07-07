using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Technician : MonoBehaviour
{
    private StateMachine _stateMachine;
    public TechnicianCarry technicianCarry;
    //public EnergyGenerator TargetEnergyGenerator { get; set; }
    public PlacedObject TargetOrganel;
    public Storage TargetStorage;
    

    private void Awake()
    {
        _stateMachine = new StateMachine();
        technicianCarry = GetComponent<TechnicianCarry>();

        var goToWork = new MoveToSelectedOrganel(this);
        var search = new SearchForStorage(this);
        var moveToSelected = new MoveToSelectedStorage(this);
        var getItemToOrganel = new GetItemToOrganel(this);
        var returnToWorkingOrganel = new ReturnToWorkingOrganel(this);
        var placeItemInOrganel = new PlaceItemInOrganel(this);



        At(goToWork, search, HasTargetOrganel());
        At(search, moveToSelected, HasTargetStorage());
        At(moveToSelected, search, StuckForOverASecond());
        At(moveToSelected, getItemToOrganel, ReachedResource());
        At(getItemToOrganel, returnToWorkingOrganel, InventoryFull());
        At(returnToWorkingOrganel, placeItemInOrganel, ReachedStockpile());
        At(placeItemInOrganel, search, () => technicianCarry.GetHoldingListCount() == 0);

        _stateMachine.SetState(goToWork);

        void At(IState to, IState from, Func<bool> condition) => _stateMachine.AddTransition(to, from, condition);

        Func<bool> HasTargetOrganel() => () => TargetOrganel != null && TargetOrganel.GetGridPosition() == technicianCarry.technicianGridPosition;
        Func<bool> HasTargetStorage() => () => TargetStorage != null;
        Func<bool> StuckForOverASecond() => () => moveToSelected.TimeStuck > 1f;


        Func<bool> ReachedResource() => () => TargetStorage != null &&
        BuildingSystem.Instance.GetGrid().GetGridObject(transform.position).GetPlacedObject() is Storage;


        Func<bool> InventoryFull() => () => (technicianCarry.GetHoldingListCount() >= technicianCarry._maxCarried);
        //((technicianCarry.GetHoldingListCount() < technicianCarry._maxCarried && technicianCarry.GetHoldingListCount() > 0 && 
        //!TargetStorage.TryGetStoredItem(new ItemSO[] { technicianCarry.miningResourceItem }, out ItemSO item)));


        Func<bool> ReachedStockpile() => () => TargetOrganel.GetGridPosition() == technicianCarry.technicianGridPosition;

    }



    private void Update() => _stateMachine.Tick();
}
