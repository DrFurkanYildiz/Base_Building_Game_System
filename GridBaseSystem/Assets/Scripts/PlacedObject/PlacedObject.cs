using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacedObject : MonoBehaviour
{
    public static PlacedObject Create(Vector3 worldPosition, Vector2Int origin, PlacedObjectTypeSO.Dir dir, PlacedObjectTypeSO placedObjectTypeSO)
    {
        Transform placedObjectTransform = Instantiate(placedObjectTypeSO.prefab, worldPosition, Quaternion.Euler(0, placedObjectTypeSO.GetRotationAngle(dir), 0));
        PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();


        placedObject.placedObjectTypeSO = placedObjectTypeSO;
        placedObject.origin = origin;
        placedObject.dir = dir;


        placedObject.Setup();
        return placedObject;
    }
    
    private PlacedObjectTypeSO placedObjectTypeSO;
    private Vector2Int origin;
    private PlacedObjectTypeSO.Dir dir;

    public void TriggerGridObjectChanged()
    {
        foreach (Vector2Int gridPosition in GetGridPositionList())
        {
            BuildingSystem.Instance.GetGrid().TriggerGridObjectChanged(gridPosition.x, gridPosition.y);
        }
    }
    protected virtual void Setup()
    {
        //Debug.Log("PlacedObject.Setup() " + transform);
    }
    
    public List<Vector2Int> GetGridPositionList()
    {
        return placedObjectTypeSO.GetGridPositionList(origin, dir);
    }
    public List<Vector2Int> GetGridRoadPositionList()
    {
        return placedObjectTypeSO.GetGridRoadPositionList(origin, dir);
    }
    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public PlacedObjectTypeSO GetPlacedObjectTypeSO()
    {
        return placedObjectTypeSO;
    }

    public Vector2Int GetGridPosition()
    {
        return origin;
    }
    public PlacedObjectTypeSO.Dir GetDir()
    {
        return dir;
    }
    public void SetOrigin(Vector2Int origin)
    {
        this.origin = origin;
    }
    public void SetDir(PlacedObjectTypeSO.Dir dir)
    {
        this.dir = dir;
    }


    public SaveObject GetSaveObject()
    {
        return new SaveObject
        {
            placedObjectTypeSO = placedObjectTypeSO,
            placedObjectTypeSOName = placedObjectTypeSO.name,
            origin = origin,
            dir = dir,
        };
    }

    [System.Serializable]
    public class SaveObject
    {
        public PlacedObjectTypeSO placedObjectTypeSO;
        public string placedObjectTypeSOName;
        public Vector2Int origin;
        public PlacedObjectTypeSO.Dir dir;

    }
}
