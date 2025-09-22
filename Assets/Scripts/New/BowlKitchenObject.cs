using System;
using System.Collections.Generic;
using UnityEngine;

public class BowlKitchenObject : KitchenObject
{
    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;
    public class OnIngredientAddedEventArgs : EventArgs
    {
        public KitchenObjectSO kitchenObjectSO;
    }

    private List<KitchenObjectSO> kitchenObjectSOList;

    private void Awake()
    {
        kitchenObjectSOList = new List<KitchenObjectSO>();
    }

    /// Copy ingredients from Pot (called by BowlCompleteVisual)
    public void CopyIngredientsFromPot(PotKitchenObject pot)
    {
        if (pot == null) return;

        kitchenObjectSOList = new List<KitchenObjectSO>(pot.GetKitchenObjectSOList());

        foreach (var ingredient in kitchenObjectSOList)
        {
            OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs { kitchenObjectSO = ingredient });
        }

        //  Debug: Show how many and which ingredients were copied
        string ingredientNames = string.Join(", ", kitchenObjectSOList.ConvertAll(i => i.name));
        Debug.Log($"[BowlKitchenObject] Copied {kitchenObjectSOList.Count} ingredients: {ingredientNames}");
    }

    public List<KitchenObjectSO> GetKitchenObjectSOList()
    {
        return kitchenObjectSOList;
    }
}
