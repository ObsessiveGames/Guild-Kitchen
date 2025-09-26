using UnityEngine;

[CreateAssetMenu(fileName = "NewOverbrewRecipe", menuName = "ScriptableObjects/OverbrewRecipeSO")]
public class OverbrewRecipeSO : ScriptableObject
{
    public KitchenObjectSO input;
    public KitchenObjectSO output;
    public float overbrewTimeMax;
}
