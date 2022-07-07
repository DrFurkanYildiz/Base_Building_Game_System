using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    private static GameAssets _i;

    public static GameAssets i
    {
        get
        {
            if (_i == null) _i = Instantiate(Resources.Load<GameAssets>("GameAssets"));
            return _i;
        }
    }

    private void Awake()
    {
        _i = this;
    }


    [System.Serializable]
    public class PlacedObjectTypeSO_Refs
    {
        public PlacedObjectTypeSO energyGenerator;
        public PlacedObjectTypeSO storage;
        public PlacedObjectTypeSO road;
        public PlacedObjectTypeSO carbohydrateResourceNode;
        //public PlacedObjectTypeSO assembler;
        //public PlacedObjectTypeSO storage;

    }

    public PlacedObjectTypeSO_Refs placedObjectTypeSO_Refs;



    [System.Serializable]
    public class ItemSO_Refs
    {

        public ItemSO energy;
        public ItemSO carbohydrate;
        //public ItemSO goldOre;
        //public ItemSO ironIngot;
        //public ItemSO goldIngot;
        //public ItemSO computer;
        //public ItemSO copperOre;
        //public ItemSO copperIngot;
        //public ItemSO microchip;

        public ItemSO any;
        public ItemSO none;
    }


    public ItemSO_Refs itemSO_Refs;



    [System.Serializable]
    public class ItemRecipeSO_Refs
    {

        public ItemRecipeSO carbohydrateToEnergyRecipe;
        //public ItemRecipeSO goldIngot;
        //public ItemRecipeSO computer;
        //public ItemRecipeSO microchip;
        //public ItemRecipeSO copperIngot;
    }


    public ItemRecipeSO_Refs itemRecipeSO_Refs;


    public Transform worldItemPrefab;
}
