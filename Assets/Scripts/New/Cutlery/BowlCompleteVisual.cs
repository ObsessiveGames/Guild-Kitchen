using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowlCompleteVisual : MonoBehaviour
{
    [SerializeField] private BowlKitchenObject bowlKitchenObject;
    [SerializeField] private GameObject filledVisual; // The object you want to toggle

    private void Start()
    {
        // Ensure visual starts hidden
        if (filledVisual != null)
        {
            filledVisual.SetActive(false);
        }

        // Subscribe to ingredient event
        bowlKitchenObject.OnIngredientAdded += BowlKitchenObject_OnIngredientAdded;
    }

    private void BowlKitchenObject_OnIngredientAdded(object sender, BowlKitchenObject.OnIngredientAddedEventArgs e)
    {
        // Activate the visual as soon as the first ingredient is added
        if (filledVisual != null && !filledVisual.activeSelf)
        {
            filledVisual.SetActive(true);
            //Debug.Log("Bowl visual activated because an ingredient was added.");
        }
    }
}
