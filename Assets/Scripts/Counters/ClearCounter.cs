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
            // There is no KitchenObject here
            if (player.HasKitchenObject())
            {
                // Player is carrying something
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
            else
            {
                // Player not carrying anything
            }
        }
        else
        {

            // There is a KitchenObject here
            KitchenObject kitchenObjectOnCounter = GetKitchenObject();

            // There is a KitchenObject here
            if (player.HasKitchenObject())
            {
                // Player is carrying something
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    // Player is holding a Plate
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        Debug.Log("this is now a pot");
                        GetKitchenObject().DestroySelf();
                    }
                }
                else if (player.GetKitchenObject().TryGetPot(out PotKitchenObject potKitchenObject))
                {
                    Debug.Log("this is now a pot");
                    // Player is holding a Pot
                    if (potKitchenObject.TryAddIngredient(kitchenObjectOnCounter.GetKitchenObjectSO()))
                    {
                        kitchenObjectOnCounter.DestroySelf();
                    }
                }
                else if (player.GetKitchenObject().TryGetBowl(out BowlKitchenObject bowlKitchenObject))
                {
                    // Player has a Bowl
                    if (kitchenObjectOnCounter.TryGetPot(out PotKitchenObject potOnCounter))
                    {
                        // Transfer ingredients from Pot to Bowl
                        bowlKitchenObject.CopyIngredientsFromPot(potOnCounter);
                        // Clear the pot now that the Bowl has taken its contents
                        potOnCounter.ClearKitchenObjects();

                        // Optional: deactivate pot visuals if using PotCompleteVisual
                        PotCompleteVisual potVisual = potOnCounter.GetComponent<PotCompleteVisual>();
                        if (potVisual != null)
                        {
                            foreach (var pair in potVisual.GetKitchenObjectSOGameObjectList())
                                pair.gameObject.SetActive(false);
                        }

                        //Debug.Log("Transferred Pot contents to Bowl!");

                        // Optional: clear pot ingredients
                        potOnCounter.SetKitchenObjects(new List<KitchenObjectSO>());
                    }
                }

                else
                {
                    // Player is not carrying Plate but something else
                    if (GetKitchenObject().TryGetPlate(out plateKitchenObject))
                    {
                        // Counter is holding a Plate
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO()))
                        {
                            Debug.Log("counter is holding plate");
                            player.GetKitchenObject().DestroySelf();
                        }
                    }
                    else if (GetKitchenObject().TryGetPot(out PotKitchenObject potOnCounter))
                    {
                        //Debug.Log("counter is holding pot");
                        // Counter is holding a Pot
                        if (potOnCounter.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO()))
                        {
                            player.GetKitchenObject().DestroySelf();
                        }
                    }
                }

            }
            else
            {
                // Player is not carrying anything
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

}