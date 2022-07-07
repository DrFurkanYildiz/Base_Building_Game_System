using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class MoveToSelectedStorage : IState
{
    private readonly Technician technician;

    private Vector3 _lastPosition = Vector3.zero;
    public float TimeStuck;
    public MoveToSelectedStorage(Technician technician)
    {
        this.technician = technician;
    }
    public void OnEnter()
    {
        TimeStuck = 0f;
        technician.transform.GetComponent<CharacterPathfindingMovementHandler>().SetTargetPosition(technician.TargetStorage.transform.position);
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
        if (Vector3.Distance(technician.transform.position, _lastPosition) <= 0f)
            TimeStuck += Time.deltaTime;

        _lastPosition = technician.transform.position;

    }
}
