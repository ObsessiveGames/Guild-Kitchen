using UnityEngine;

[CreateAssetMenu(fileName = "NewPotionRecipe", menuName = "ScriptableObjects/PotionRecipeSO")]
public class PotionRecipeSO : ScriptableObject
{
    public KitchenObjectSO input;
    public KitchenObjectSO output;
    public float brewingTimeMax;
}
