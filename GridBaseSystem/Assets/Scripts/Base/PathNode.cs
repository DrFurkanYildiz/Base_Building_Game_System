using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    public enum PathNodeEnum
    {
        None,
        Wall,
    }

    private Grid<PathNode> grid;
    private PlacedObject placedObject;
    private PathNodeEnum pathNodeEnum;
    private NodeHandle nodeHandle;
    //private bool isBuild;
    public int x;
    public int y;

    #region PathfindingSystemParam

    public int gCost;
    public int hCost;
    public int fCost;

    public PathNode cameFromNode;
    public PathNode roadFromNode;
    public bool isWalkable;

    #endregion


    public PathNode(Grid<PathNode> grid, int x, int y)
    {
        this.x = x;
        this.y = y;
        this.grid = grid;
        placedObject = null;
        nodeHandle = NodeHandle.Create(grid, x, y);
        //isBuild = true;
        isWalkable = true;
    }
    public NodeHandle GetNodeHandle()
    {
        return nodeHandle;
    }
    public Vector2Int GetNodePosition()
    {
        return new Vector2Int(x, y);
    }

    public void SetPlacedObject(PlacedObject placedObject)
    {
        this.placedObject = placedObject;
        grid.TriggerGridObjectChanged(x, y);
    }
    public void ClearPlacedObject()
    {
        placedObject = null;
        grid.TriggerGridObjectChanged(x, y);
    }
    public PlacedObject GetPlacedObject()
    {
        return placedObject;
    }
    public bool CanBuild()
    {
        return placedObject ? false : true;
    }
    //public void SetIsBuild(bool isBuild)
    //{
    //    this.isBuild = isBuild;
    //    grid.TriggerGridObjectChanged(x, y);
    //}
    internal void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public void SetIsWalkable(bool isWalkable)
    {
        this.isWalkable = isWalkable;
        grid.TriggerGridObjectChanged(x, y);
    }

    public override string ToString()
    {
        if (placedObject == null)
        {
            return x + "," + y;
        }
        else
        {
            return isWalkable.ToString() + "," + placedObject.GetPlacedObjectTypeSO().nameString;
        }
    }

    [Serializable]
    public class SaveObject
    {
        public PathNodeEnum pathNodeEnum;
        public int x;
        public int y;
        public bool isWalkable;
        //public bool isBuild;
    }
    public SaveObject Save()
    {
        return new SaveObject
        {
            pathNodeEnum = pathNodeEnum,
            x = x,
            y = y,
            isWalkable = isWalkable,
            //isBuild = isBuild
        };
    }
    public void Load(SaveObject saveObject)
    {
        pathNodeEnum = saveObject.pathNodeEnum;
        isWalkable = saveObject.isWalkable;
        //isBuild = saveObject.isBuild;
    }
}
