using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : PlacedObject
{
    [SerializeField] private List<Transform> roadVisualList = new List<Transform>();
    private PlacedObjectTypeSO.Dir roadDirection;

    protected override void Setup()
    {
        BuildingSystem.Instance.RoadChangeVisual();
    }

    public void ChangeRoadVisual()
    {
        if (!GetPlacedObjectTypeSO().isRoadObject)
            return;

        bool down = false;
        bool right = false;
        bool up = false;
        bool left = false;

        Vector2Int vectorDown = new Vector2Int(GetGridPosition().x, GetGridPosition().y - 1);
        Vector2Int vectorRight = new Vector2Int(GetGridPosition().x + 1, GetGridPosition().y);
        Vector2Int vectorUp = new Vector2Int(GetGridPosition().x, GetGridPosition().y + 1);
        Vector2Int vectorLeft = new Vector2Int(GetGridPosition().x - 1, GetGridPosition().y);

        foreach (var item in NeighbourList())
        {
            if (item.x == vectorDown.x && item.y == vectorDown.y)
                down = true;
            if (item.x == vectorRight.x && item.y == vectorRight.y)
                right = true;
            if (item.x == vectorUp.x && item.y == vectorUp.y)
                up = true;
            if (item.x == vectorLeft.x && item.y == vectorLeft.y)
                left = true;
        }

        if (down) roadDirection = PlacedObjectTypeSO.Dir.Down;
        if (right) roadDirection = PlacedObjectTypeSO.Dir.Right;
        if (up) roadDirection = PlacedObjectTypeSO.Dir.Up;
        if (left) roadDirection = PlacedObjectTypeSO.Dir.Left;

        switch (NeighbourList().Count)
        {
            case 0:
                roadVisualList.ForEach(r => r.gameObject.SetActive(false));
                roadVisualList[0].gameObject.SetActive(true);
                break;
            case 1:
                roadVisualList.ForEach(r => r.gameObject.SetActive(false));
                roadVisualList[1].gameObject.SetActive(true);
                roadVisualList[1].gameObject.transform.rotation = Quaternion.Euler(-90, 0, GetPlacedObjectTypeSO().GetRotationAngle(roadDirection) + 90);
                break;
            case 2:
                roadVisualList.ForEach(r => r.gameObject.SetActive(false));
                if (down && up)
                {
                    roadVisualList[5].gameObject.SetActive(true);
                    roadVisualList[5].gameObject.transform.rotation = Quaternion.Euler(-90, 0, 90);
                }
                if (right && left)
                {
                    roadVisualList[5].gameObject.SetActive(true);
                    roadVisualList[5].gameObject.transform.rotation = Quaternion.Euler(-90, 0, 0);
                }
                if (right && down)
                {
                    roadVisualList[2].gameObject.SetActive(true);
                    roadVisualList[2].gameObject.transform.rotation = Quaternion.Euler(-90, 0, 0);
                }
                if (left && down)
                {
                    roadVisualList[2].gameObject.SetActive(true);
                    roadVisualList[2].gameObject.transform.rotation = Quaternion.Euler(-90, 0, 90);
                }
                if (right && up)
                {
                    roadVisualList[2].gameObject.SetActive(true);
                    roadVisualList[2].gameObject.transform.rotation = Quaternion.Euler(-90, 0, 270);
                }
                if (left && up)
                {
                    roadVisualList[2].gameObject.SetActive(true);
                    roadVisualList[2].gameObject.transform.rotation = Quaternion.Euler(-90, 0, 180);
                }
                break;
            case 3:
                roadVisualList.ForEach(r => r.gameObject.SetActive(false));
                if (right && down && up)
                {
                    roadVisualList[3].gameObject.SetActive(true);
                    roadVisualList[3].gameObject.transform.rotation = Quaternion.Euler(-90, 0, 0);
                }
                if (right && down && left)
                {
                    roadVisualList[3].gameObject.SetActive(true);
                    roadVisualList[3].gameObject.transform.rotation = Quaternion.Euler(-90, 0, 90);
                }
                if (left && down && up)
                {
                    roadVisualList[3].gameObject.SetActive(true);
                    roadVisualList[3].gameObject.transform.rotation = Quaternion.Euler(-90, 0, 180);
                }
                if (left && up && right)
                {
                    roadVisualList[3].gameObject.SetActive(true);
                    roadVisualList[3].gameObject.transform.rotation = Quaternion.Euler(-90, 0, 270);
                }
                break;
            case 4:
                roadVisualList.ForEach(r => r.gameObject.SetActive(false));
                roadVisualList[4].gameObject.SetActive(true);
                break;
        }
    }

    private List<PathNode> NeighbourList()
    {
        List<PathNode> neighbours = new List<PathNode>();
        foreach (PathNode neighbour in BuildingSystem.Instance.GetNeighbour(BuildingSystem.Instance.GetNode(GetGridPosition().x, GetGridPosition().y)))
        {
            if (neighbour.GetPlacedObject() == null)
            {
                neighbours.Remove(neighbour);
                continue;
            }

            if (neighbour.GetPlacedObject().GetPlacedObjectTypeSO().isRoadObject)
            {
                if (!neighbours.Contains(neighbour))
                    neighbours.Add(neighbour);
            }
            else
            {
                if (BuildingSystem.Instance.GetGrid().GetGridObject(neighbour.GetNodePosition().x, neighbour.GetNodePosition().y).isWalkable)
                    if (!neighbours.Contains(neighbour))
                        neighbours.Add(neighbour);
            }
        }
        return neighbours;
    }
}
