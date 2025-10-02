using System;
using System.Collections.Generic;
using UnityEngine;

public class PotCompleteVisual : MonoBehaviour
{
    [Serializable]
    public struct KitchenObjectSO_GameObject
    {
        public KitchenObjectSO kitchenObjectSO;
        public GameObject gameObject;
    }

    [SerializeField] private PotKitchenObject potKitchenObject;
    [SerializeField] private List<KitchenObjectSO_GameObject> kitchenObjectSOGameObjectList;

    // This will be activated when cooking is complete and deactivated when pot is cleared
    [SerializeField] private GameObject cookedCompleteVisual;

    private void Start()
    {
        // Subscribe to ingredient added event
        potKitchenObject.OnIngredientAdded += PotKitchenObject_OnIngredientAdded;

        // Subscribe to pot cleared event (e.g. after transferring to bowl)
        potKitchenObject.OnPotCleared += PotKitchenObject_OnPotCleared;

        // Disable all ingredient visuals at start
        foreach (KitchenObjectSO_GameObject kitchenObjectSOGameObject in kitchenObjectSOGameObjectList)
        {
            kitchenObjectSOGameObject.gameObject.SetActive(false);
        }

        // Ensure cooked completion visual is disabled at start
        if (cookedCompleteVisual != null)
        {
            cookedCompleteVisual.SetActive(false);
        }
    }

    private void PotKitchenObject_OnIngredientAdded(object sender, PotKitchenObject.OnIngredientAddedEventArgs e)
    {
        // Enable the visual for the ingredient that was added
        foreach (KitchenObjectSO_GameObject kitchenObjectSOGameObject in kitchenObjectSOGameObjectList)
        {
            if (kitchenObjectSOGameObject.kitchenObjectSO == e.kitchenObjectSO)
            {
                kitchenObjectSOGameObject.gameObject.SetActive(true);
            }
        }
    }

    /// Call this method after cooking is done to replace raw visuals with cooked visuals
    /// <param name="rawIngredient">The raw KitchenObjectSO</param>
    /// <param name="cookedIngredient">The cooked KitchenObjectSO</param>
    public void ReplaceRawWithCooked(KitchenObjectSO rawIngredient, KitchenObjectSO cookedIngredient)
    {
        GameObject rawGO = null;
        GameObject cookedGO = null;

        // Find the raw and cooked GameObjects from the mapping list
        foreach (var obj in kitchenObjectSOGameObjectList)
        {
            if (obj.kitchenObjectSO == rawIngredient)
                rawGO = obj.gameObject;
            if (obj.kitchenObjectSO == cookedIngredient)
                cookedGO = obj.gameObject;
        }

        // Deactivate raw, activate cooked visuals
        if (rawGO != null) rawGO.SetActive(false);
        if (cookedGO != null) cookedGO.SetActive(true);

        // Activate the cooked completion visual
        if (cookedCompleteVisual != null)
        {
            cookedCompleteVisual.SetActive(true);
        }
    }

    public List<KitchenObjectSO_GameObject> GetKitchenObjectSOGameObjectList()
    {
        return kitchenObjectSOGameObjectList;
    }

    private void PotKitchenObject_OnPotCleared(object sender, EventArgs e)
    {
        // Set all ingredient visuals to inactive
        foreach (KitchenObjectSO_GameObject kitchenObjectSOGameObject in kitchenObjectSOGameObjectList)
        {
            kitchenObjectSOGameObject.gameObject.SetActive(false);
        }

        // Deactivate the cooked completion visual
        if (cookedCompleteVisual != null)
        {
            cookedCompleteVisual.SetActive(false);
        }
    }
}
