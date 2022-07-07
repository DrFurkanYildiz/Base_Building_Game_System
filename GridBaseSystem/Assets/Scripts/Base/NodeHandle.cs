using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeHandle : MonoBehaviour
{
    public bool isSelected;
    public static NodeHandle Create(Grid<PathNode> grid, int x, int y)
    {
        Transform NodeTransform = Instantiate(BuildingSystem.Instance.SelectionVisual);
        NodeHandle nodeHandle = NodeTransform.GetComponent<NodeHandle>();
        NodeTransform.SetParent(BuildingSystem.Instance.transform, false);

        Vector3 pos = grid.GetWorldPosition(x, y);
        Vector3 sca = new Vector3(grid.GetCellSize(), .1f, grid.GetCellSize());

        NodeTransform.transform.position = pos + sca / 2;
        NodeTransform.transform.localScale = sca;


        return nodeHandle;
    }
    private void Update()
    {
        GetComponent<MeshRenderer>().enabled = isSelected;
    }
}
