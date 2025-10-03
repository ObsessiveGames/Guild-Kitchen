using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperBagIconsUI : MonoBehaviour
{
    [SerializeField] private PaperBagKitchenObject paperBagKitchenObject;
    [SerializeField] private Transform iconTemplate;

    private void Awake()
    {
        iconTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        paperBagKitchenObject.OnIngredientAdded += PaperBagKitchenObject_OnIngredientAdded;
    }

    private void PaperBagKitchenObject_OnIngredientAdded(object sender, PaperBagKitchenObject.OnIngredientAddedEventArgs e)
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

        foreach (KitchenObjectSO kitchenObjectSO in paperBagKitchenObject.GetKitchenObjectSOList())
        {
            Transform iconTransform = Instantiate(iconTemplate, transform);
            iconTransform.gameObject.SetActive(true);
            iconTransform.GetComponent<PlateIconsSingleUI>().SetKitchenObjectSO(kitchenObjectSO);
        }
    }
}
