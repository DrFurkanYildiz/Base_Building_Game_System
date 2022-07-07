using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class MoveToSelectedOrganel : IState
{
    private readonly Technician technician;

    public MoveToSelectedOrganel(Technician technician)
    {
        this.technician = technician;
    }
    public void OnEnter() { }

    public void OnExit() { }

    public void Tick()
    {
        if (BuildingSystem.Instance.selectedObject is EnergyGenerator)
            technician.TargetOrganel = BuildingSystem.Instance.selectedObject;


        if (technician.TargetOrganel != null)
            technician.transform.GetComponent<CharacterPathfindingMovementHandler>().SetTargetPosition(technician.TargetOrganel.transform.position);

    }
}
