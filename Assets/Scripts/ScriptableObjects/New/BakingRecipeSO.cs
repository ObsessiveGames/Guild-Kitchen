using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/BakingRecipeSO")]
public class BakingRecipeSO : ScriptableObject
{
    public KitchenObjectSO input;       // Raw ingredient
    public KitchenObjectSO output;      // Baked result
    public float bakingTimeMax;         // Time until baking is done
}
