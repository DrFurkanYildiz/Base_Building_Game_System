using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class PlacedObjectTypeSO : ScriptableObject
{
    public static Dir GetNextDir(Dir dir)
    {
        switch (dir)
        {
            default:
            case Dir.Down: return Dir.Left;
            case Dir.Left: return Dir.Up;
            case Dir.Up: return Dir.Right;
            case Dir.Right: return Dir.Down;
        }
    }

    public enum Dir
    {
        Down,
        Left,
        Up,
        Right,
    }

    public string nameString;
    public Transform prefab;
    public int width;
    public int height;
    public bool isWalkingObject;
    public bool isRoadObject;
    public bool isMovableObject;

    public int GetRotationAngle(Dir dir)
    {
        switch (dir)
        {
            default:
            case Dir.Down: return 0;
            case Dir.Left: return 90;
            case Dir.Up: return 180;
            case Dir.Right: return 270;
        }
    }

    public Vector2Int GetRotationOffset(Dir dir)
    {
        switch (dir)
        {
            default:
            case Dir.Down: return new Vector2Int(0, 0);
            case Dir.Left: return new Vector2Int(0, width);
            case Dir.Up: return new Vector2Int(width, height);
            case Dir.Right: return new Vector2Int(height, 0);
        }
    }
    public List<Vector2Int> GetGridPositionList(Vector2Int offset, Dir dir)
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>();
        switch (dir)
        {
            default:
            case Dir.Down:
            case Dir.Up:
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        gridPositionList.Add(offset + new Vector2Int(x, y));
                    }
                }
                break;
            case Dir.Left:
            case Dir.Right:
                for (int x = 0; x < height; x++)
                {
                    for (int y = 0; y < width; y++)
                    {
                        gridPositionList.Add(offset + new Vector2Int(x, y));
                    }
                }
                break;
        }
        return gridPositionList;
    }
    public List<Vector2Int> GetGridRoadPositionList(Vector2Int offset, Dir dir)
    {
        List<Vector2Int> gridRoadPositionList = new List<Vector2Int>();
        switch (dir)
        {
            default:
            case Dir.Down:
                for (int x = 0; x < width; x++)
                {
                    gridRoadPositionList.Add(offset + new Vector2Int(x, 0));
                }
                break;
            case Dir.Up:
                for (int x = 0; x < width; x++)
                {
                    gridRoadPositionList.Add(offset + new Vector2Int(x, height - 1));
                }
                break;
            case Dir.Left:
                for (int y = 0; y < width; y++)
                {
                    gridRoadPositionList.Add(offset + new Vector2Int(0, y));
                }
                break;
            case Dir.Right:
                for (int y = 0; y < width; y++)
                {
                    gridRoadPositionList.Add(offset + new Vector2Int(height - 1, y));
                }
                break;
        }
        return gridRoadPositionList;
    }
    
}