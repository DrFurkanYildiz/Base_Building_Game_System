using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GathererManager : MonoBehaviour
{
    public static GathererManager Instance { get; private set; }

    public GameObject gathererPrefab;
    public GameObject technicianPrefab;
    public List<Gatherer> gatherers = new List<Gatherer>();
    public List<Technician> technicians = new List<Technician>();


    private void Awake()
    {
        Instance = this;

    }

    void Start()
    {
        Transform go = Instantiate(gathererPrefab, Vector3.one * 5f, Quaternion.identity).transform;
        var gatherer = go.GetComponent<Gatherer>();
        gatherers.Add(gatherer);

        Transform go1 = Instantiate(gathererPrefab, new Vector3(45, 0, 45), Quaternion.identity).transform;
        var gatherer1 = go1.GetComponent<Gatherer>();
        gatherers.Add(gatherer1);

        Transform tec = Instantiate(technicianPrefab, Vector3.one * 5f, Quaternion.identity).transform;
        var technician = tec.GetComponent<Technician>();
        technicians.Add(technician);
    }
    public int EmptyWorkerAmount()
    {
        int index = 0;
        foreach (var item in gatherers)
        {
            if (item.GetStateMachine().GetState() is WorkerStates.EmptyWorker)
                index++;
        }
        return index;
    }
    public Gatherer GetEmptyWorker()
    {
        foreach (var item in gatherers)
        {
            if (item.GetStateMachine().GetState() is WorkerStates.EmptyWorker)
                return item;
        }
        return null;
    }
}
