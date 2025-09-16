using UnityEngine;

[CreateAssetMenu()]
public class OverboilRecipeSO : ScriptableObject
{
    public KitchenObjectSO input;
    public KitchenObjectSO output;
    public float overboilTimeMax;
}
