using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerStates
{
    public Gatherer gatherer;

    public class EmptyWorker : WorkerStates, IState
    {
        public EmptyWorker(Gatherer gatherer)
        {
            this.gatherer = gatherer;
        }


        public void OnEnter()
        {
        }

        public void OnExit()
        {
        }

        public void Tick()
        {
            gatherer.transform.GetComponent<CharacterPathfindingMovementHandler>().SetTargetPosition(new Vector3(0, 0, 70));
        }
    }
    public class MoveToSelectedResource : WorkerStates, IState
    {
        public MoveToSelectedResource(Gatherer gatherer)
        {
            this.gatherer = gatherer;
        }


        public void OnEnter()
        {
            gatherer.transform.GetComponent<CharacterPathfindingMovementHandler>().SetTargetPosition(gatherer.TargetResourceNode.transform.position);
        }

        public void OnExit()
        {

        }

        public void Tick()
        {
        }
    }
    public class HarvestResource : WorkerStates, IState
    {
        public HarvestResource(Gatherer gatherer)
        {
            this.gatherer = gatherer;
        }

        public void OnEnter()
        {
            gatherer.gathererMining.miningResourceItem = gatherer.TargetResourceNode.GetResourceItem();
        }

        public void OnExit() { }

        public void Tick()
        {
            if (gatherer.TargetResourceNode != null)
            {
                // Type of object that can be dropped
                ItemSO[] dropFilterItemSO = new ItemSO[] { gatherer.gathererMining.miningResourceItem };
                //if (gatherer.gathererMining is IItemStorage)
                //{
                //    dropFilterItemSO = (gatherer.gathererMining as IItemStorage).GetItemSOThatCanStore();
                //}
                //if (gatherer.gathererMining is IWorldItemSlot)
                //{
                //    dropFilterItemSO = (gatherer.gathererMining as IWorldItemSlot).GetItemSOThatCanStore();
                //}

                //// Combine Drop and Grab filters
                //dropFilterItemSO = ItemSO.GetCombinedFilter(new ItemSO[] { gatherer.gathererMining.grabFilterItemSO }, dropFilterItemSO);


                //if (ItemSO.IsItemSOInFilter(GameAssets.i.itemSO_Refs.none, dropFilterItemSO))
                //{
                //    //Harvestta zaten işlemeyecek bu bölüm depodan Jeneratöre taşırken kontrol edilir.
                //    // Cannot drop any item, so dont grab anything
                //    //Debug.Log("OnExit");
                //   // OnExit();
                //}

                // Is Grab PlacedObject a Item Storage?
                if (gatherer.gathererMining is IItemStorage)
                {
                    IItemStorage mining = gatherer.gathererMining as IItemStorage;

                    if (mining.TryGetStoredItem(dropFilterItemSO, out ItemSO itemScriptableObject))
                    {
                        if (gatherer.TargetResourceNode.TryAvailableResource())
                        {
                            WorldItem worldItem = WorldItem.Create(gatherer.gathererMining.gathererGridPosition, itemScriptableObject);
                            gatherer.gathererMining.holdingItemList.Add(worldItem);
                        }
                        //gatherer.gathererMining.holdingItem = WorldItem.Create(gatherer.gathererMining.gathererGridPosition, itemScriptableObject);
                        //gatherer.gathererMining.holdingItem.SetGridPosition(gatherer.gathererMining.gathererGridPosition);

                    }
                }

            }
        }
    }
    public class ReturnToStockpile : WorkerStates, IState
    {
        public ReturnToStockpile(Gatherer gatherer)
        {
            this.gatherer = gatherer;
        }

        public void OnEnter()
        {

        }

        public void OnExit()
        {

        }

        public void Tick()
        {
            foreach (var item in BuildingSystem.Instance.placedBuildingList)
            {
                if (item is Storage)
                {
                    gatherer.TargetStorage = item as Storage;
                    gatherer.transform.GetComponent<CharacterPathfindingMovementHandler>().SetTargetPosition(item.transform.position);
                }
            }
        }
    }
    public class PlaceResourcesInStockpile : WorkerStates, IState
    {
        public PlaceResourcesInStockpile(Gatherer gatherer)
        {
            this.gatherer = gatherer;
        }

        public void OnEnter()
        {
            gatherer.gathererMining.miningResourceItem = null;
        }

        public void OnExit() { }

        public void Tick()
        {
            if (gatherer.TargetStorage != null)
            {
                if (gatherer.TargetStorage is IItemStorage)
                {
                    IItemStorage itemStorage = gatherer.TargetStorage as IItemStorage;


                    if (itemStorage.TryStoreItem(gatherer.gathererMining.holdingItemList[0].GetItemSO(), gatherer.gathererMining.GetHoldingListCount()))
                    {
                        for (int i = 0; i < gatherer.gathererMining.holdingItemList.Count; i++)
                        {
                            gatherer.gathererMining.holdingItemList[i].DestroySelf();
                        }
                        gatherer.gathererMining.holdingItemList.Clear();
                    }
                }
            }
        }
    }
}
