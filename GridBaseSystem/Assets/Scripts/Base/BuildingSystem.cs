using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSystem : MonoBehaviour
{
    public static BuildingSystem Instance { get; private set; }

    public event EventHandler OnSelectedChanged;
    public event EventHandler OnObjectPlaced;

    private Grid<PathNode> grid;

    //[SerializeField] private List<PlacedObjectTypeSO> placedObjectTypeSOList = null;
    [SerializeField]private PlacedObjectTypeSO placedObjectTypeSO;
    private PlacedObjectTypeSO.Dir dir;
    [SerializeField]private bool isDemolishActive;
    [SerializeField]private bool isMovableActive;
    [SerializeField] private bool isSection;
    [SerializeField] private bool isRoadSection;
    [SerializeField] private bool isOpenPatfinding;
    public bool IsOpenPatfinding { get { return isOpenPatfinding; } private set { } }




    [SerializeField] private Transform visual;
    public Transform SelectionVisual;

    /// <summary>
    /// Deneme
    /// </summary>

    public PlacedObject selectedObject = null;
    //public ResourceNode selectedResourceNode = null;
    public List<PlacedObject> placedBuildingList = new List<PlacedObject>();




    private List<Vector2Int> paths = new List<Vector2Int>();
    private List<Vector2Int> roadPaths = new List<Vector2Int>();
    private List<PathNode> selectedPathNodeList = new List<PathNode>();
    private List<PathNode> roadPathNodeList = new List<PathNode>();
    private Vector3 startPos;
    private Vector3 startPositionRoad;


    #region PATFINDING PARAM
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;
    private List<PathNode> openList;
    private List<PathNode> closedList;
    #endregion


    #region ROAD PATFINDING PARAM
    private const int ROAD_MOVE_STRAIGHT_COST = 10;
    private const int ROAD_MOVE_DIAGONAL_COST = 24;
    private List<PathNode> roadOpenList;
    private List<PathNode> roadClosedList;
    private bool isRoadPlaced;
    [SerializeField] private List<Road> allRoads = new List<Road>();
    #endregion

    private void Awake()
    {
        Instance = this;

        int gridWidth = 20;
        int gridHeight = 20;
        float cellSize = 10f;


        grid = new Grid<PathNode>(gridWidth, gridHeight, cellSize, new Vector3(0, 0, 0), (Grid<PathNode> g, int x, int y) => new PathNode(g, x, y));

        placedObjectTypeSO = null;// placedObjectTypeSOList[0];
    }
    private void Update()
    {

        #region PATFINDING TEST
        //  PATFINDING TEST

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!isOpenPatfinding)
                isOpenPatfinding = true;
            else
                isOpenPatfinding = false;
        }

        if (isOpenPatfinding)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePosition = Mouse3D.GetMouseWorldPosition();
                GetGrid().GetXZ(mousePosition, out int x, out int y);
                List<PathNode> path = FindPath(0, 0, x, y);

                if (path != null)
                {
                    for (int i = 0; i < path.Count - 1; i++)
                    {
                        Debug.DrawLine(new Vector3(path[i].x, 0, path[i].y) * 10f + Vector3.one * 5f,
                            new Vector3(path[i + 1].x, 0, path[i + 1].y) * 10f + Vector3.one * 5f, Color.black, 2f);
                    }
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                Vector3 mouseWorldPosition = Mouse3D.GetMouseWorldPosition();
                GetGrid().GetXZ(mouseWorldPosition, out int x, out int y);
                GetNode(x, y).SetIsWalkable(!GetNode(x, y).isWalkable);
            }
        }
        #endregion


        if (Input.GetKeyDown(KeyCode.Alpha1)) { placedObjectTypeSO = GameAssets.i.placedObjectTypeSO_Refs.energyGenerator; RefreshSelectedObjectType(); RefreshVisual(); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { placedObjectTypeSO = GameAssets.i.placedObjectTypeSO_Refs.road; RefreshSelectedObjectType(); RefreshVisual(); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { placedObjectTypeSO = GameAssets.i.placedObjectTypeSO_Refs.carbohydrateResourceNode; RefreshSelectedObjectType(); RefreshVisual(); }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { placedObjectTypeSO = GameAssets.i.placedObjectTypeSO_Refs.storage; RefreshSelectedObjectType(); RefreshVisual(); }
        

        if (Input.GetKeyDown(KeyCode.Alpha0)) { SetDemolishActive(); }
        
        if (Input.GetKeyDown(KeyCode.Alpha9)) { isMovableActive = true; }

        if (Input.GetKeyDown(KeyCode.Alpha6)) { isSection = true; paths.Clear(); }
        if (Input.GetKeyDown(KeyCode.Alpha5)) { isRoadSection = true; roadPaths.Clear(); }

        HandleBuildingSelection();

        HandleNormalObjectPlacement();
        HandleMovableObjectPlacement();
        HandleDemolish();
        HandleSelectionPlacement();
        HandleRoadSelectionPlacement();
    }
    private void LateUpdate()
    {
        if (visual != null)
        {

            if (Input.GetKeyDown(KeyCode.R))
            {
                dir = PlacedObjectTypeSO.GetNextDir(dir);
            }

            Vector3 targetPosition = GetMouseWorldSnappedPosition();
            targetPosition.y = 1f;
            visual.transform.position = Vector3.Lerp(visual.transform.position, targetPosition, Time.deltaTime * 15f);

            visual.transform.rotation = Quaternion.Lerp(visual.transform.rotation, GetPlacedObjectRotation(), Time.deltaTime * 15f);
        }


        if (isRoadPlaced)
        {
            isRoadPlaced = false;
            for (int i = 0; i < allRoads.Count; i++)
            {
                if (allRoads[i] != null)
                    allRoads[i].ChangeRoadVisual();
                else
                    allRoads.RemoveAt(i);
            }
        }

    }
    private void HandleBuildingSelection()
    {
        if (Input.GetMouseButtonDown(0) && GetPlacedObjectTypeSO() == null && !Helpers.IsPointerOverUI())
        {
            // Not building anything
            if (grid.GetGridObject(Mouse3D.GetMouseWorldPosition()) != null)
            {
                PlacedObject placedObject = grid.GetGridObject(Mouse3D.GetMouseWorldPosition()).GetPlacedObject();
                if (placedObject != null)
                {
                    // Clicked on something
                    if (placedObject is ResourceNode)
                    {
                        selectedObject = placedObject as ResourceNode;
                        ResourceNodeUI.Instance.Show(placedObject as ResourceNode);
                    }
                    else if (placedObject is EnergyGenerator)
                    {
                        selectedObject = placedObject as EnergyGenerator;
                    }
                    else if(placedObject is Storage)
                    {
                        selectedObject = placedObject as Storage;
                        StorageUI.Instance.Show(placedObject as Storage);
                    }
                }
                else
                {
                    selectedObject = null;
                }
            }
        }
    }

    #region ROAD SELECTİON METOT
    private void HandleRoadSelectionPlacement()
    {
        if (isRoadSection)
        {
            if (Input.GetMouseButtonDown(0))
            {
                startPositionRoad = Mouse3D.GetMouseWorldPosition();
            }
            if (Input.GetMouseButton(0))
            {
                if (Input.GetMouseButtonDown(1))
                    DeselectGridRoadPath();
                else
                {
                    UpdateNeighbourListesi(Mouse3D.GetMouseWorldPosition());
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                ReleaseSelectionRoadGrid();
            }
        }
    }
    private void UpdateNeighbourListesi(Vector3 currentPosition)
    {
        grid.GetXZ(startPositionRoad, out int startX, out int startY);
        grid.GetXZ(currentPosition, out int currentX, out int currentY);


        PathNode currentNode = grid.GetGridObject(currentX, currentY);
        if (!currentNode.isWalkable)
            return;

        roadPathNodeList = FindRoad(startX, startY, currentX, currentY);

        {
            foreach (var item in GridAllPathNode())
            {
                if (roadPathNodeList.Contains(item))
                    item.GetNodeHandle().isSelected = true;
                else
                    item.GetNodeHandle().isSelected = false;
            }
        }

    }
    private void ReleaseSelectionRoadGrid()
    {
        for (int i = 0; i < roadPathNodeList.Count; i++)
        {
            var sPath = new Vector2Int(roadPathNodeList[i].x, roadPathNodeList[i].y);
            if (!roadPaths.Contains(sPath))
            {
                roadPaths.Add(sPath);
                //selectedPathNodeList[i].SetIsBuild(true);
            }

            CreateRoadPlacedObject(grid, sPath, GameAssets.i.placedObjectTypeSO_Refs.road);
        }
        DeselectGridRoadPath();
    }
    private void DeselectGridRoadPath()
    {
        roadPathNodeList.Clear();
        roadPaths.Clear();
        GridAllPathNode().ForEach(p => p.GetNodeHandle().isSelected = false);
        isRoadSection = false;
    }
    public void RoadChangeVisual()
    {
        isRoadPlaced = true;
    }

    public List<PathNode> FindRoad(int startX, int startY, int endX, int endY)
    {
        PathNode startNode = grid.GetGridObject(startX, startY);
        PathNode endNode = grid.GetGridObject(endX, endY);

        roadOpenList = new List<PathNode>() { startNode };
        roadClosedList = new List<PathNode>();

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                PathNode pathNode = grid.GetGridObject(x, y);
                pathNode.gCost = int.MaxValue;
                pathNode.CalculateFCost();
                pathNode.roadFromNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateRoadDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        while (roadOpenList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(roadOpenList);
            if (currentNode == endNode)
            {
                //Final Node
                return CalculateRoad(endNode);
            }

            roadOpenList.Remove(currentNode);
            roadClosedList.Add(currentNode);

            foreach (PathNode neigbourNode in GetNeighbourList(currentNode, true))
            {
                if (roadClosedList.Contains(neigbourNode)) continue;
                if (!neigbourNode.isWalkable)
                {
                    roadClosedList.Add(neigbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.gCost + CalculateRoadDistanceCost(currentNode, neigbourNode);
                if (tentativeGCost < neigbourNode.gCost)
                {
                    neigbourNode.roadFromNode = currentNode;
                    neigbourNode.gCost = tentativeGCost;
                    neigbourNode.hCost = CalculateRoadDistanceCost(neigbourNode, endNode);
                    neigbourNode.CalculateFCost();

                    if (!roadOpenList.Contains(neigbourNode))
                    {
                        roadOpenList.Add(neigbourNode);
                    }
                }
            }
        }
        return null;
    }
    private List<PathNode> CalculateRoad(PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();
        path.Add(endNode);
        PathNode currentNode = endNode;
        while (currentNode.roadFromNode != null)
        {
            path.Add(currentNode.roadFromNode);
            currentNode = currentNode.roadFromNode;
        }
        path.Reverse();
        return path;
    }
    private int CalculateRoadDistanceCost(PathNode a, PathNode b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return ROAD_MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + ROAD_MOVE_STRAIGHT_COST * remaining;
    }

    #endregion

    #region SELECTİON METOTS

    private void HandleSelectionPlacement()
    {
        if (isSection)
        {
            if (Input.GetMouseButtonDown(0))
            {
                startPos = Mouse3D.GetMouseWorldPosition();
            }
            if (Input.GetMouseButton(0))
            {
                UpdateSelectionGrid(Mouse3D.GetMouseWorldPosition());
            }
            if (Input.GetMouseButtonUp(0))
            {
                ReleaseSelectionGrid();
            }
        }
    }
    private List<PathNode> GridAllPathNode()
    {
        List<PathNode> paths = new List<PathNode>();
        for (int i = 0; i < grid.GetWidth(); i++)
        {
            for (int j = 0; j < grid.GetHeight(); j++)
            {
                paths.Add(GetNode(i, j));
            }
        }
        return paths;
    }
    private List<PathNode> GetSelectionNeighbourList(PathNode startNode, PathNode currentNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();

        int xLen = currentNode.x - startNode.x;
        int yLen = currentNode.y - startNode.y;

        for (int i = startNode.x; i < startNode.x + xLen + 1; i++)
        {
            for (int j = startNode.y; j < startNode.y + yLen + 1; j++)
            {
                neighbourList.Add(GetNode(i, j));
            }
        }
        return neighbourList;
    }
    private void DeselectGridPath()
    {
        selectedPathNodeList.Clear();
        paths.Clear();
        GridAllPathNode().ForEach(p => p.GetNodeHandle().isSelected = false);
        isSection = false;
    }
    private void UpdateSelectionGrid(Vector3 curMousePos)
    {
        float width = curMousePos.x - startPos.x;
        float height = curMousePos.z - startPos.z;

        Vector3 selectionPosition;
        Vector3 selectionScale;

        selectionPosition = startPos + new Vector3(width / 2f, 0f, height / 2f);
        selectionScale = new Vector3(Mathf.Abs(width), 0, Mathf.Abs(height));

        Vector3 min = selectionPosition - selectionScale / 2f;
        Vector3 max = selectionPosition + selectionScale / 2f;

        grid.GetXZ(min, out int minX, out int minZ);
        grid.GetXZ(max, out int maxX, out int maxZ);

        PathNode currentNode = GetNode(maxX, maxZ);
        PathNode startNode = GetNode(minX, minZ);

        selectedPathNodeList = GetSelectionNeighbourList(startNode, currentNode);

        foreach (var item in GridAllPathNode())
        {
            if (selectedPathNodeList.Contains(item))
                item.GetNodeHandle().isSelected = true;
            else
                item.GetNodeHandle().isSelected = false;
        }
    }
    private void ReleaseSelectionGrid()
    {
        if (selectedPathNodeList.Count >= 9) //Min 3x3
        {
            for (int i = 0; i < selectedPathNodeList.Count; i++)
            {
                var sPath = new Vector2Int(selectedPathNodeList[i].x, selectedPathNodeList[i].y);
                if (!paths.Contains(sPath))
                {
                    paths.Add(sPath);
                    //selectedPathNodeList[i].SetIsBuild(true);
                }
            }
            DeselectGridPath();
        }
        else
        {
            DeselectGridPath();
        }
    }


    #endregion

    private void HandleNormalObjectPlacement()
    {
        if (Input.GetMouseButton(0) && placedObjectTypeSO != null && !isMovableActive)
        {
            Vector3 mousePosition = Mouse3D.GetMouseWorldPosition();
            grid.GetXZ(mousePosition, out int x, out int z);

            Vector2Int placedObjectOrigin = new Vector2Int(x, z);

            CreatePlacedObject(grid, placedObjectOrigin, placedObjectTypeSO, dir, out PlacedObject placedObject);


            //........................................Deneme
            if (placedObject is Storage)
                placedBuildingList.Add(placedObject as Storage);
            else if (placedObject is EnergyGenerator)
                placedBuildingList.Add(placedObject as EnergyGenerator);
            else if (placedObject is ResourceNode)
                placedBuildingList.Add(placedObject as ResourceNode);
        }
    }
    private void HandleMovableObjectPlacement()
    {
        if (isMovableActive)
        {
            if (Input.GetMouseButtonDown(1))
            {
                Vector3 mousePosition = Mouse3D.GetMouseWorldPosition();
                PlacedObject placedObject = grid.GetGridObject(mousePosition).GetPlacedObject();

                if (placedObject != null && placedObject.GetPlacedObjectTypeSO().isMovableObject)
                {

                    visual = placedObject.transform;
                    placedObjectTypeSO = placedObject.GetPlacedObjectTypeSO();
                    visual.GetComponent<PlacedObject>().transform.rotation = Quaternion.Euler(0, visual.GetComponent<PlacedObject>().GetPlacedObjectTypeSO().GetRotationAngle(dir), 0);

                    List<Vector2Int> gridPositionList = visual.GetComponent<PlacedObject>().GetGridPositionList();

                    foreach (Vector2Int gridPosition in gridPositionList)
                    {
                        GetNode(gridPosition.x, gridPosition.y).SetIsWalkable(true);
                        grid.GetGridObject(gridPosition.x, gridPosition.y).ClearPlacedObject();
                    }

                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (visual != null)
                {
                    var visualObject = visual.GetComponent<PlacedObject>();
                    Vector3 mousePosition = Mouse3D.GetMouseWorldPosition();

                    grid.GetXZ(mousePosition, out int x, out int z);
                    Vector2Int placedObjectOrigin = new Vector2Int(x, z);

                    Vector2Int rotationOffset = visualObject.GetPlacedObjectTypeSO().GetRotationOffset(dir);
                    Vector3 placedObjectWorldPosition = grid.GetWorldPosition(placedObjectOrigin.x, placedObjectOrigin.y) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();

                    visualObject.SetDir(dir);
                    visualObject.SetOrigin(placedObjectOrigin);



                    List<Vector2Int> gridPositionList = visualObject.GetPlacedObjectTypeSO().GetGridPositionList(placedObjectOrigin, visualObject.GetDir());
                    List<Vector2Int> gridRoadPositionList = placedObjectTypeSO.GetGridRoadPositionList(placedObjectOrigin, dir);

                    bool canBuild = true;

                    foreach (Vector2Int gridPosition in gridPositionList)
                    {
                        //bool isValidPosition = grid.IsValidGridPositionWithPadding(gridPosition);
                        bool isValidPosition = grid.IsValidGridPosition(gridPosition);
                        if (!isValidPosition)
                        {
                            // Not valid
                            canBuild = false;
                            break;
                        }
                        if (!grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild())
                        {
                            canBuild = false;
                            break;
                        }
                    }


                    if (canBuild)
                    {
                        foreach (Vector2Int gridPosition in gridPositionList)
                        {
                            grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(visualObject);
                            bool isWalkingObject = visualObject.GetPlacedObjectTypeSO().isWalkingObject;
                            //GetNode(gridPosition.x, gridPosition.y).SetIsWalkable(!isWalkingObject ? !GetNode(gridPosition.x, gridPosition.y).isWalkable : GetNode(gridPosition.x, gridPosition.y).isWalkable);
                            GetNode(gridPosition.x, gridPosition.y).SetIsWalkable(isWalkingObject);
                        }

                        gridRoadPositionList.ForEach(gridRoad => GetNode(gridRoad.x, gridRoad.y).SetIsWalkable(true));

                        visualObject.transform.position = placedObjectWorldPosition;
                        visualObject.transform.rotation = Quaternion.Euler(0, placedObjectTypeSO.GetRotationAngle(dir), 0);

                        visual = null;
                        placedObjectTypeSO = null;
                        isMovableActive = false;
                    }
                    else
                    {
                        // Cannot build here
                    }
                }
            }
        }
    }
    private void HandleDemolish()
    {
        if (isDemolishActive && Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Mouse3D.GetMouseWorldPosition();
            PlacedObject placedObject = grid.GetGridObject(mousePosition).GetPlacedObject();
            if (placedObject != null) // && placedObject.GetPlacedObjectTypeSO().nameString != "Wall"
            {
                if (placedObject is Road)
                {
                    allRoads.Remove(placedObject as Road);
                    isRoadPlaced = true;
                }

                // Demolish
                placedObject.DestroySelf();
                

                List<Vector2Int> gridPositionList = placedObject.GetGridPositionList();
                List<Vector2Int> gridRoadPositionList = placedObject.GetGridRoadPositionList();

                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    GetNode(gridPosition.x, gridPosition.y).SetIsWalkable(true);
                    grid.GetGridObject(gridPosition.x, gridPosition.y).ClearPlacedObject();
                }
            }
        }
    }
    public void CreatePlacedObject(Grid<PathNode> grid, Vector2Int placedObjectOrigin, PlacedObjectTypeSO placedObjectTypeSO, PlacedObjectTypeSO.Dir dir, out PlacedObject placedObject)
    {
        List<Vector2Int> gridPositionList = placedObjectTypeSO.GetGridPositionList(placedObjectOrigin, dir);
        List<Vector2Int> gridRoadPositionList = placedObjectTypeSO.GetGridRoadPositionList(placedObjectOrigin, dir);
        List<Vector2Int> fullObjectList = new List<Vector2Int>();

        for (int i = 0; i < gridPositionList.Count; i++)
        {
            if (!gridRoadPositionList.Contains(gridPositionList[i]))
                fullObjectList.Add(gridPositionList[i]);
        }


        bool canBuild = true;
        foreach (Vector2Int gridPosition in gridPositionList)
        {
            bool isValidPosition = grid.IsValidGridPosition(gridPosition);
            if (!isValidPosition)
            {
                // Not valid
                canBuild = false;
                break;
            }
            //if (!grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild())
            //{
            //    canBuild = false;
            //    break;
            //}
        }

        foreach (Vector2Int item in fullObjectList)
        {
            if (!grid.GetGridObject(item.x, item.y).CanBuild())
            {
                canBuild = false;
                break;
            }
        }

        foreach (Vector2Int item in gridRoadPositionList)
        {
            var currentPlaced = grid.GetGridObject(item.x, item.y).GetPlacedObject();
            if(currentPlaced != null)
            {
                if (!currentPlaced.GetPlacedObjectTypeSO().isRoadObject && !currentPlaced.GetGridRoadPositionList().Contains(item))
                {
                    canBuild = false;
                    break;
                }
                else
                {
                    if (currentPlaced.GetPlacedObjectTypeSO().isRoadObject && canBuild)
                    {
                        currentPlaced.DestroySelf();
                    }
                }
            }
        }


        if (canBuild && !isMovableActive)
        {
            Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
            Vector3 placedObjectWorldPosition = grid.GetWorldPosition(placedObjectOrigin.x, placedObjectOrigin.y) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();

            placedObject = PlacedObject.Create(placedObjectWorldPosition, placedObjectOrigin, dir, placedObjectTypeSO);
            
            foreach (Vector2Int gridPosition in gridPositionList)
            {
                grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);
                bool isWalkingObject = placedObject.GetPlacedObjectTypeSO().isWalkingObject;
                GetNode(gridPosition.x, gridPosition.y).SetIsWalkable(isWalkingObject);
            }

            gridRoadPositionList.ForEach(gridRoad => GetNode(gridRoad.x, gridRoad.y).SetIsWalkable(true));

            DeselectObjectType();

            OnObjectPlaced?.Invoke(placedObject, EventArgs.Empty);

        }
        else
        {
            // Cannot build here
            placedObject = null;
        }
    }
    public void CreateRoadPlacedObject(Grid<PathNode> grid, Vector2Int placedObjectOrigin, PlacedObjectTypeSO placedObjectTypeSO)
    {
        List<Vector2Int> gridPositionList = placedObjectTypeSO.GetGridPositionList(placedObjectOrigin, dir);

        bool canBuild = true;
        foreach (Vector2Int gridPosition in gridPositionList)
        {
            bool isValidPosition = grid.IsValidGridPosition(gridPosition);
            if (!isValidPosition)
            {
                // Not valid
                canBuild = false;
                break;
            }
            if (!grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild())
            {
                canBuild = false;
                break;
            }
            if(!roadPathNodeList.TrueForAll(p => p.isWalkable))
            {
                canBuild = false;
                break;
            }

        }

        if (canBuild)
        {
            Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
            Vector3 placedObjectWorldPosition = grid.GetWorldPosition(placedObjectOrigin.x, placedObjectOrigin.y) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();

            var placedObject = PlacedObject.Create(placedObjectWorldPosition, placedObjectOrigin, dir, placedObjectTypeSO);

            allRoads.Add(placedObject as Road);

            foreach (Vector2Int gridPosition in gridPositionList)
            {
                grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);
            }

            DeselectObjectType();
            OnObjectPlaced?.Invoke(placedObject, EventArgs.Empty);
        }
    }

    private void DeselectObjectType()
    {
        placedObjectTypeSO = null;
        isDemolishActive = false;
        isMovableActive = false;
        RefreshSelectedObjectType();
    }
    private void RefreshSelectedObjectType()
    {
        OnSelectedChanged?.Invoke(this, EventArgs.Empty);
        OnSelectedChanged += Instance_OnSelectedChanged;
    }
    private void SetDemolishActive()
    {
        if (!isDemolishActive)
        {
            placedObjectTypeSO = null;
            isDemolishActive = true;
            isMovableActive = false;
            RefreshSelectedObjectType();
        }
        else
        {
            isDemolishActive = false;
        }
    }
    

    public Vector3 GetMouseWorldSnappedPosition()
    {
        Vector3 mousePosition = Mouse3D.GetMouseWorldPosition();
        grid.GetXZ(mousePosition, out int x, out int z);

        if (placedObjectTypeSO != null)
        {
            Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
            Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();
            return placedObjectWorldPosition;
        }
        else
        {
            return mousePosition;
        }
    }

    public Quaternion GetPlacedObjectRotation()
    {
        if (placedObjectTypeSO != null)
        {
            return Quaternion.Euler(0, placedObjectTypeSO.GetRotationAngle(dir), 0);
        }
        else
        {
            return Quaternion.identity;
        }
    }

    public PlacedObjectTypeSO GetPlacedObjectTypeSO()
    {
        return placedObjectTypeSO;
    }

    //public PlacedObjectTypeSO GetPlacedObjectTypeSOFromName(string placedObjectName)
    //{
    //    foreach (PlacedObjectTypeSO placedObjectTypeSO in GameAssets.i.placedObjectTypeSO_Refs)
    //    {
    //        if (placedObjectTypeSO.name == placedObjectName)
    //        {
    //            return placedObjectTypeSO;
    //        }
    //    }
    //    return null;
    //}

    public Vector3 GetWorldPosition(Vector2Int gridPosition)
    {
        return grid.GetWorldPosition(gridPosition.x, gridPosition.y);
    }
    public Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        grid.GetXZ(worldPosition, out int x, out int y);
        Vector2Int gridPosition = new Vector2Int(x, y);
        if (!grid.IsValidGridPosition(gridPosition))
            return Vector2Int.zero;

        return gridPosition;
    }

    private void Instance_OnSelectedChanged(object sender, EventArgs e)
    {
        RefreshVisual();
    }
    private void RefreshVisual()
    {
        if (visual != null)
        {
            Destroy(visual.gameObject);
            visual = null;
        }

        PlacedObjectTypeSO placedObjectTypeSO = GetPlacedObjectTypeSO();

        if (placedObjectTypeSO != null)
        {
            visual = Instantiate(placedObjectTypeSO.prefab, Vector3.zero, Quaternion.identity);
            visual.parent = transform;
            visual.localPosition = Vector3.zero;
            visual.localEulerAngles = Vector3.zero;
        }
    }


    #region PATFINDING METOTS

    public Grid<PathNode> GetGrid()
    {
        return grid;
    }
    public List<Vector3> FindPath(Vector3 startWorldPosition, Vector3 endWorldPosition)
    {
        grid.GetXZ(startWorldPosition, out int startX, out int startZ);
        grid.GetXZ(endWorldPosition, out int endX, out int endZ);

        List<PathNode> path = FindPath(startX, startZ, endX, endZ);
        if (path == null)
        {
            return null;
        }
        else
        {
            List<Vector3> vectorPath = new List<Vector3>();
            foreach (PathNode pathNode in path)
            {
                vectorPath.Add(new Vector3(pathNode.x, 0, pathNode.y) * grid.GetCellSize() + Vector3.one * grid.GetCellSize() * .5f);
            }
            return vectorPath;
        }
    }
    public List<PathNode> FindPath(int startX, int startY, int endX, int endY, bool isGetRoad = false)
    {
        PathNode startNode = grid.GetGridObject(startX, startY);
        PathNode endNode = grid.GetGridObject(endX, endY);

        openList = new List<PathNode>() { startNode };
        closedList = new List<PathNode>();

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                PathNode pathNode = grid.GetGridObject(x, y);
                pathNode.gCost = int.MaxValue;
                pathNode.CalculateFCost();
                pathNode.cameFromNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(openList);
            if (currentNode == endNode)
            {
                //Final Node
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (PathNode neigbourNode in GetNeighbourList(currentNode, isGetRoad))
            {
                if (closedList.Contains(neigbourNode)) continue;
                if (!neigbourNode.isWalkable)
                {
                    closedList.Add(neigbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neigbourNode);
                if (tentativeGCost < neigbourNode.gCost)
                {
                    neigbourNode.cameFromNode = currentNode;
                    neigbourNode.gCost = tentativeGCost;
                    neigbourNode.hCost = CalculateDistanceCost(neigbourNode, endNode);
                    neigbourNode.CalculateFCost();

                    if (!openList.Contains(neigbourNode))
                    {
                        openList.Add(neigbourNode);
                    }
                }
            }
        }
        //Bütün haritaya bakıldı yol bulunamadı..
        Debug.Log("Gidilemez");
        return null;
    }
    public List<PathNode> GetNeighbour(PathNode node)
    {
        return GetNeighbourList(node, true);
    }
    private List<PathNode> GetNeighbourList(PathNode currentNode, bool isGetRoad)
    {
        List<PathNode> neighbourList = new List<PathNode>();
        //Eğer çapraz hareket kapatılması gerkiyor ise..
        //Sol Aşağı ..Sol Yukarı.. Sağ Aşağı.. Sağ Yukarı Node ekleme bölümleri kapatılmalı.

        if (currentNode.x - 1 >= 0)
        {
            //Sol..
            neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y));
            if (!isGetRoad)
            {
                //Sol Aşağı
                if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y - 1));
                //Sol Yukarı
                if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y + 1));
            }
        }
        if (currentNode.x + 1 < grid.GetWidth())
        {
            //Sağ..
            neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y));
            if (!isGetRoad)
            {
                //Sağ Aşağı
                if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y - 1));
                //Sağ Yukarı
                if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y + 1));
            }
        }
        //Aşağı
        if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x, currentNode.y - 1));
        //Yukarı
        if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x, currentNode.y + 1));

        return neighbourList;
    }

    public PathNode GetNode(int x, int y)
    {
        return grid.GetGridObject(x, y);
    }

    private List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();
        path.Add(endNode);
        PathNode currentNode = endNode;
        while (currentNode.cameFromNode != null)
        {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }
        path.Reverse();
        return path;
    }
    private int CalculateDistanceCost(PathNode a, PathNode b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;

    }
    private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostNode = pathNodeList[0];
        for (int i = 1; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = pathNodeList[i];
            }
        }
        return lowestFCostNode;
    }

    #endregion
}
