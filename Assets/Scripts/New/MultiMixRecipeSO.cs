// Assets/Scripts/Kitchen/Mixing/MultiMixRecipeSO.cs
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MultiMixRecipe", menuName = "Kitchen Chaos/Recipe/Multi-Mix")]
public class MultiMixRecipeSO : ScriptableObject
{
    [Serializable]
    public class IngredientEntry
    {
        public KitchenObjectSO ingredient;
        [Min(1)] public int count = 1;
    }

    [Tooltip("Unordered list; counts matter.")]
    public List<IngredientEntry> inputs = new List<IngredientEntry>();

    [Tooltip("Resulting single ingredient (e.g., Batter, DoughBall, Sauce).")]
    public KitchenObjectSO output;

    [Header("Mixing")]
    [Tooltip("Click-based: how many presses; Time-based: used to compute progress if you prefer ticks.")]
    public int mixProgressMax = 6;

    [Tooltip("Time-based alternative (seconds) if using auto-mix mode on the counter.")]
    public float mixSeconds = 3f;

    /// <summary>Returns a dictionary {ingredientSO -> requiredCount}</summary>
    public Dictionary<KitchenObjectSO, int> BuildRequiredCounts()
    {
        var dict = new Dictionary<KitchenObjectSO, int>();
        foreach (var e in inputs)
        {
            if (e.ingredient == null || e.count <= 0) continue;
            if (!dict.ContainsKey(e.ingredient)) dict[e.ingredient] = 0;
            dict[e.ingredient] += e.count;
        }
        return dict;
    }
}