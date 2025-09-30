using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPotionRecipe", menuName = "ScriptableObjects/PotionRecipeSO")]
public class PotionRecipeSO : ScriptableObject
{
    public KitchenObjectSO [] inputList;   // Ingredients needed
    public KitchenObjectSO output;            // Resulting potion
    public float brewingTimeMax;              // Time to brew
    public string recipeName;                 // Optional: name for UI/feedback
}
