using System.Collections.Generic;
using System;
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


    private void Start()
    {
        potKitchenObject.OnIngredientAdded += PotKitchenObject_OnIngredientAdded;

        foreach (KitchenObjectSO_GameObject kitchenObjectSOGameObject in kitchenObjectSOGameObjectList)
        {
            kitchenObjectSOGameObject.gameObject.SetActive(false);
        }
    }

    private void PotKitchenObject_OnIngredientAdded(object sender, PotKitchenObject.OnIngredientAddedEventArgs e)
    {
        foreach (KitchenObjectSO_GameObject kitchenObjectSOGameObject in kitchenObjectSOGameObjectList)
        {
            if (kitchenObjectSOGameObject.kitchenObjectSO == e.kitchenObjectSO)
            {
                kitchenObjectSOGameObject.gameObject.SetActive(true);
            }
        }
    }

}
