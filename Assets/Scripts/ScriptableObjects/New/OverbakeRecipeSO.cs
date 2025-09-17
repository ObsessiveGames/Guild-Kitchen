using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/OverbakeRecipeSO")]
public class OverbakeRecipeSO : ScriptableObject
{
    public KitchenObjectSO input;       // Baked ingredient
    public KitchenObjectSO output;      // Overbaked (burned) result
    public float overbakeTimeMax;       // Time until food is burned
}
