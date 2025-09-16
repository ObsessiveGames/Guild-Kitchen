using UnityEngine;

[CreateAssetMenu()]
public class BoilingRecipeSO : ScriptableObject
{
    public KitchenObjectSO input;
    public KitchenObjectSO output;
    public float boilingTimeMax;
}
