using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class ReturnToWorkingOrganel : IState
{
    private readonly Technician technician;

    public ReturnToWorkingOrganel(Technician technician)
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
        technician.transform.GetComponent<CharacterPathfindingMovementHandler>().SetTargetPosition(technician.TargetOrganel.transform.position);
    }
}
