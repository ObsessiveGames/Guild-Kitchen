using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            // No KitchenObject on counter
            if (player != null && player.HasKitchenObject())
            {
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
            return;
        }

        // Counter has a KitchenObject
        KitchenObject kitchenObjectOnCounter = GetKitchenObject();

        if (player != null && player.HasKitchenObject())
        {
            // --- PLAYER IS CARRYING SOMETHING ---
            KitchenObject playerObject = player.GetKitchenObject();

            // --- PLAYER HOLDS PLATE ---
            if (playerObject.TryGetPlate(out PlateKitchenObject plateKitchenObject))
            {
                if (kitchenObjectOnCounter != null && kitchenObjectOnCounter.GetKitchenObjectSO() != null)
                {
                    if (plateKitchenObject.TryAddIngredient(kitchenObjectOnCounter.GetKitchenObjectSO()))
                    {
                        kitchenObjectOnCounter.DestroySelf();
                    }
                }
                return;
            }

            // --- PLAYER HOLDS POT ---
            if (playerObject.TryGetPot(out PotKitchenObject potKitchenObject))
            {
                // Pot (in hand) -> Bowl (on counter)
                if (kitchenObjectOnCounter.TryGetBowl(out BowlKitchenObject bowlOnCounter))
                {
                    if (potKitchenObject.GetKitchenObjectSOList().Count > 0)
                    {
                        bowlOnCounter.CopyIngredientsFromPot(potKitchenObject);
                        potKitchenObject.ClearKitchenObjects();
                    }
                    return;
                }

                // Pot (in hand) <- Ingredient (on counter)
                if (kitchenObjectOnCounter != null && kitchenObjectOnCounter.GetKitchenObjectSO() != null)
                {
                    if (potKitchenObject.TryAddIngredient(kitchenObjectOnCounter.GetKitchenObjectSO()))
                    {
                        kitchenObjectOnCounter.DestroySelf();
                    }
                }
                return;
            }

            // --- PLAYER HOLDS BOWL ---
            if (playerObject.TryGetBowl(out BowlKitchenObject bowlKitchenObject))
            {
                // Bowl (in hand) <- Pot (on counter)
                if (kitchenObjectOnCounter.TryGetPot(out PotKitchenObject potOnCounter))
                {
                    if (potOnCounter.GetKitchenObjectSOList().Count > 0)
                    {
                        bowlKitchenObject.CopyIngredientsFromPot(potOnCounter);
                        potOnCounter.ClearKitchenObjects();

                        // Optional: disable pot visuals
                        PotCompleteVisual potVisual = potOnCounter.GetComponent<PotCompleteVisual>();
                        if (potVisual != null)
                        {
                            foreach (var pair in potVisual.GetKitchenObjectSOGameObjectList())
                            {
                                pair.gameObject.SetActive(false);
                            }
                        }

                        potOnCounter.SetKitchenObjects(new List<KitchenObjectSO>());
                    }
                }
                return;
            }

            // --- PLAYER HOLDS SOMETHING ELSE ---
            if (kitchenObjectOnCounter.TryGetPlate(out PlateKitchenObject plateOnCounter))
            {
                if (playerObject != null && playerObject.GetKitchenObjectSO() != null)
                {
                    if (plateOnCounter.TryAddIngredient(playerObject.GetKitchenObjectSO()))
                    {
                        playerObject.DestroySelf();
                    }
                }
            }
            else if (kitchenObjectOnCounter.TryGetPot(out PotKitchenObject potOnCounter2))
            {
                if (playerObject != null && playerObject.GetKitchenObjectSO() != null)
                {
                    if (potOnCounter2.TryAddIngredient(playerObject.GetKitchenObjectSO()))
                    {
                        playerObject.DestroySelf();
                    }
                }
            }
        }
        else
        {
            // --- PLAYER EMPTY-HANDED ---
            kitchenObjectOnCounter.SetKitchenObjectParent(player);
        }
    }
}
