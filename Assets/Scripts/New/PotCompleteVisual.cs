using System;
using System.Collections.Generic;
using UnityEngine;

public class PotCompleteVisual : MonoBehaviour
{


    [Serializable]
    public struct KitchenObjectSO_GameObject
    {

        public KitchenObjectSO kitchenObjectSO;
        public GameObject gameObject;

    }


    [SerializeField] private PotKitchenObject potKitchenObject;
    [SerializeField] private List<KitchenObjectSO_GameObject> kitchenObjectSOGameObjectList;


    private void Start()
    {
        potKitchenObject.OnIngredientAdded += PotKitchenObject_OnIngredientAdded;

        // To clear visual after pot-to-bowl transfer
        potKitchenObject.OnPotCleared += PotKitchenObject_OnPotCleared;

        foreach (KitchenObjectSO_GameObject kitchenObjectSOGameObject in kitchenObjectSOGameObjectList)
        {
            kitchenObjectSOGameObject.gameObject.SetActive(false);
        }
    }

    private void PotKitchenObject_OnIngredientAdded(object sender, PotKitchenObject.OnIngredientAddedEventArgs e)
    {
        foreach (KitchenObjectSO_GameObject kitchenObjectSOGameObject in kitchenObjectSOGameObjectList)
        {
            if (kitchenObjectSOGameObject.kitchenObjectSO == e.kitchenObjectSO)
            {
                kitchenObjectSOGameObject.gameObject.SetActive(true);
            }
        }
    }

    /// Call this method after cooking is done to replace raw visuals with cooked visuals
    /// <param name="rawIngredient">The raw KitchenObjectSO</param>
    /// <param name="cookedIngredient">The cooked KitchenObjectSO</param>
    
    
    public void ReplaceRawWithCooked(KitchenObjectSO rawIngredient, KitchenObjectSO cookedIngredient)
    {
        GameObject rawGO = null;
        GameObject cookedGO = null;

        foreach (var obj in kitchenObjectSOGameObjectList)
        {
            if (obj.kitchenObjectSO == rawIngredient)
                rawGO = obj.gameObject;
            if (obj.kitchenObjectSO == cookedIngredient)
                cookedGO = obj.gameObject;
        }

        if (rawGO != null) rawGO.SetActive(false);
        if (cookedGO != null) cookedGO.SetActive(true);
    }

    public List<KitchenObjectSO_GameObject> GetKitchenObjectSOGameObjectList()
    {
        return kitchenObjectSOGameObjectList;
    }

    private void PotKitchenObject_OnPotCleared(object sender, EventArgs e)
    {
        // SET ALL INGREDIENT VISUALS TO INACTIVE
        foreach (KitchenObjectSO_GameObject kitchenObjectSOGameObject in kitchenObjectSOGameObjectList)
        {
            kitchenObjectSOGameObject.gameObject.SetActive(false);
        }
    }
}