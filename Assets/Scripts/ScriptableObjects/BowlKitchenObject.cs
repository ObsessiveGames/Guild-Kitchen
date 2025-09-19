// Assets/Scripts/Kitchen/Mixing/BowlKitchenObject.cs
using System.Collections.Generic;
using UnityEngine;

public class BowlKitchenObject : KitchenObject
{
    [Header("Contents (unordered multiset)")]
    [SerializeField] private List<KitchenObjectSO> contents = new List<KitchenObjectSO>();

    [Header("Limits")]
    [SerializeField] private int maxUniqueIngredients = 5; // optional guard
    [SerializeField] private int maxTotalCount = 10;       // optional guard

    /// <summary>Returns a copy of current contents (one entry per item, duplicates allowed).</summary>
    public List<KitchenObjectSO> GetContents() => new List<KitchenObjectSO>(contents);

    /// <summary>Attempt to add a loose ingredient (not a plate, not a bowl).</summary>
    public bool TryAddIngredient(KitchenObjectSO ingredient)
    {
        if (ingredient == null) return false;
        if (contents.Count >= maxTotalCount) return false;

        // Unique-count cap (optional)
        int unique = 0;
        var uniqueSet = new HashSet<KitchenObjectSO>();
        foreach (var c in contents) uniqueSet.Add(c);
        unique = uniqueSet.Count;
        if (!uniqueSet.Contains(ingredient) && unique >= maxUniqueIngredients) return false;

        contents.Add(ingredient);
        return true;
    }

    /// <summary>Remove all contents (used before placing result).</summary>
    public void ClearContents()
    {
        contents.Clear();
    }    /// <summary>
         /// After mixing, we store exactly one result item inside the bowl.
         /// Returns false if bowl already contains something unexpected.
         /// </summary>
    public bool SetMixedResult(KitchenObjectSO result)
    {
        if (result == null) return false;
        contents.Clear();
        contents.Add(result);
        return true;
    }

    /// <summary>
    /// If bowl currently holds a single, final result ingredient, transfer it onto a plate and clear.
    /// </summary>
    public bool TryPourOnto(PlateKitchenObject plate)
    {
        if (plate == null) return false;
        if (contents.Count != 1) return false; // only support pouring the final single result
        var resultSO = contents[0];
        if (plate.TryAddIngredient(resultSO))
        {
            contents.Clear();
            return true;
        }
        return false;
    }

    /// <summary>Utility: Count multiset of current contents.</summary>
    public Dictionary<KitchenObjectSO, int> BuildCounts()
    {
        var dict = new Dictionary<KitchenObjectSO, int>();
        foreach (var so in contents)
        {
            if (!dict.ContainsKey(so)) dict[so] = 0;
            dict[so]++;
        }
        return dict;
    }
}