using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItem : MonoBehaviour
{
    public static WorldItem Create(Vector2Int gridPosition, ItemSO itemScriptableObject)
    {
        Transform worldItemTransform = Instantiate(GameAssets.i.worldItemPrefab, BuildingSystem.Instance.GetWorldPosition(gridPosition), Quaternion.identity);

        WorldItem worldItem = worldItemTransform.GetComponent<WorldItem>();
        worldItem.SetGridPosition(gridPosition);
        worldItem.itemSO = itemScriptableObject;

        return worldItem;
    }

    private Vector2Int gridPosition;
    private bool hasAlreadyMoved;
    [SerializeField]private ItemSO itemSO;


    private void Start()
    {
        //transform.Find("ItemVisual").Find("itemSprite").GetComponent<SpriteRenderer>().sprite = itemSO.sprite;
    }


    public void SetGridPosition(Vector2Int gridPosition)
    {
        this.gridPosition = gridPosition;
    }
    public ItemSO GetItemSO()
    {
        return itemSO;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
