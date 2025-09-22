using System;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotIconsUI : MonoBehaviour
{
    [SerializeField] private PotKitchenObject potKitchenObject;
    [SerializeField] private Transform iconTemplate;

    private void Awake()
    {
        iconTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        potKitchenObject.OnIngredientAdded += PotKitchenObject_OnIngredientAdded;

        // this is to clear Ingredients
        potKitchenObject.OnPotCleared += PotKitchenObject_OnPotCleared;
    }

    private void PotKitchenObject_OnIngredientAdded(object sender, PotKitchenObject.OnIngredientAddedEventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        foreach (Transform child in transform)
        {
            if (child == iconTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach (KitchenObjectSO kitchenObjectSO in potKitchenObject.GetKitchenObjectSOList())
        {
            Transform iconTransform = Instantiate(iconTemplate, transform);
            iconTransform.gameObject.SetActive(true);
            iconTransform.GetComponent<PotIconsSingleUI>().SetKitchenObjectSO(kitchenObjectSO);
        }
    }

    private void PotKitchenObject_OnPotCleared(object sender, EventArgs e)
    {
        UpdateVisual(); // refresh icons when pot is emptied
    }
}
