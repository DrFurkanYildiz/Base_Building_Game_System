using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Save();
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            Load();
        }
    }


    private void Save()
    {
        List<PathNode.SaveObject> pathNodeObjectSaveObjectList = new List<PathNode.SaveObject>();


        List<PlacedObjectSaveObjectArray> placedObjectSaveObjectArrayList = new List<PlacedObjectSaveObjectArray>();
        List<PlacedObject.SaveObject> placedObjectSaveObjectList = new List<PlacedObject.SaveObject>();
        List<PlacedObject> savedPlacedObjectList = new List<PlacedObject>();




        for (int x = 0; x < BuildingSystem.Instance.GetGrid().GetWidth(); x++)
        {
            for (int y = 0; y < BuildingSystem.Instance.GetGrid().GetHeight(); y++)
            {
                PathNode pathNode = BuildingSystem.Instance.GetGrid().GetGridObject(x, y);
                pathNodeObjectSaveObjectList.Add(pathNode.Save());

                PlacedObject placedObject = BuildingSystem.Instance.GetGrid().GetGridObject(x, y).GetPlacedObject();
                if (placedObject != null && !savedPlacedObjectList.Contains(placedObject))
                {
                    // Save object
                    savedPlacedObjectList.Add(placedObject);
                    placedObjectSaveObjectList.Add(placedObject.GetSaveObject());
                }
            }
        }

        PlacedObjectSaveObjectArray placedObjectSaveObjectArray = new PlacedObjectSaveObjectArray { placedObjectSaveObjectArray = placedObjectSaveObjectList.ToArray() };
        placedObjectSaveObjectArrayList.Add(placedObjectSaveObjectArray);

        SaveObject saveObject = new SaveObject
        {
            pathNodeObjectSaveObjectArray = pathNodeObjectSaveObjectList.ToArray(),
            placedObjectSaveObjectArrayArray = placedObjectSaveObjectArrayList.ToArray()
        };

        string json = JsonUtility.ToJson(saveObject);

        PlayerPrefs.SetString("HouseBuildingSystemSave", json);
        SaveLoadSystem.Save("HouseBuildingSystemSave", json, false);

        Debug.Log("Saved!");
    }

    private void Load()
    {
        string json = PlayerPrefs.GetString("HouseBuildingSystemSave");
        json = SaveLoadSystem.Load("HouseBuildingSystemSave_11");

        SaveObject saveObject = JsonUtility.FromJson<SaveObject>(json);

        foreach (PathNode.SaveObject pathNodeSaveObject in saveObject.pathNodeObjectSaveObjectArray)
        {
            PathNode pathNode = BuildingSystem.Instance.GetGrid().GetGridObject(pathNodeSaveObject.x, pathNodeSaveObject.y);
            pathNode.Load(pathNodeSaveObject);
        }


        foreach (PlacedObject.SaveObject placedObjectSaveObject in saveObject.placedObjectSaveObjectArrayArray[0].placedObjectSaveObjectArray)
        {
            if (placedObjectSaveObject != null)
            {
                BuildingSystem.Instance.CreatePlacedObject(BuildingSystem.Instance.GetGrid(), placedObjectSaveObject.origin, placedObjectSaveObject.placedObjectTypeSO, placedObjectSaveObject.dir, out PlacedObject placedObject);
                
            }
            else
            {
                Debug.Log("Boş!");
            }
        }
        Debug.Log("Load!");
    }


    [Serializable]
    public class SaveObject
    {
        public PathNode.SaveObject[] pathNodeObjectSaveObjectArray;
        public PlacedObjectSaveObjectArray[] placedObjectSaveObjectArrayArray;
    }

    [Serializable]
    public class PlacedObjectSaveObjectArray
    {
        public PlacedObject.SaveObject[] placedObjectSaveObjectArray;
    }
}
