using System;
using System.Collections.Generic;
using UnityEngine;

public class PotKitchenObject : KitchenObject
{
    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;
    public class OnIngredientAddedEventArgs : EventArgs
    {
        public KitchenObjectSO kitchenObjectSO;
    }

    public event EventHandler OnPotCleared;


    [SerializeField] private List<KitchenObjectSO> validKitchenObjectSOList;
    [SerializeField] private List<BoilingRecipeSO> boilingRecipeSOList;
    [SerializeField] private PotCompleteVisual potCompleteVisual;



    private List<KitchenObjectSO> kitchenObjectSOList;

    private void Awake()
    {
        kitchenObjectSOList = new List<KitchenObjectSO>();
    }

    public List<KitchenObjectSO> GetKitchenObjectSOList()
    {
        return kitchenObjectSOList;
    }

    /// Try adding an ingredient to the pot. Returns true if successful.
    public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO) {
        if (!validKitchenObjectSOList.Contains(kitchenObjectSO))
            return false; // Not valid

        if (kitchenObjectSOList.Contains(kitchenObjectSO))
            return false; // Already added

        kitchenObjectSOList.Add(kitchenObjectSO);

        OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs {
            kitchenObjectSO = kitchenObjectSO
        });

        return true;
    }


    /// Get a copy of the ingredients currently in the pot
    public List<KitchenObjectSO> GetKitchenObjects(){
        return kitchenObjectSOList;
    }


    /// Replace the pot's ingredient list with a new list
    public void SetKitchenObjects(List<KitchenObjectSO> newIngredients) {
        kitchenObjectSOList = newIngredients;
    }


    /// Trigger cooking of all raw ingredients in the pot
    public void CookIngredients() {
        if (kitchenObjectSOList.Count == 0) return;

        List<KitchenObjectSO> newIngredients = new List<KitchenObjectSO>();

        foreach (KitchenObjectSO ingredient in kitchenObjectSOList) {
            BoilingRecipeSO recipe = GetBoilingRecipeForInput(ingredient);
            if (recipe != null) {
                newIngredients.Add(recipe.output);
                potCompleteVisual.ReplaceRawWithCooked(ingredient, recipe.output);
            } else {
                newIngredients.Add(ingredient);
            }
        }

        kitchenObjectSOList = newIngredients;
        //Debug.Log("Pot ingredients cooked!");
    }


    /// Find the boiling recipe for a given ingredient
    private BoilingRecipeSO GetBoilingRecipeForInput(KitchenObjectSO input) {
        foreach (BoilingRecipeSO recipe in boilingRecipeSOList) {
            if (recipe.input == input)
                return recipe;
        }

        return null;
    }

    /// Clear all ingredients from the pot (used when player takes them with a bowl)
    public void ClearKitchenObjects()
    {
        kitchenObjectSOList.Clear();
        Debug.Log("Pot has been emptied!");
        OnPotCleared?.Invoke(this, EventArgs.Empty);
    }
}
