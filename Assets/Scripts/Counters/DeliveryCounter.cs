using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryCounter : BaseCounter
{
    public static DeliveryCounter Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public override void Interact(Player player)
    {
        if (player.HasKitchenObject())
        {
            if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
            {
                // Deliver Plate
                DeliveryManager.Instance.DeliverRecipe(plateKitchenObject);
                player.GetKitchenObject().DestroySelf();
            }
            else if (player.GetKitchenObject().TryGetBowl(out BowlKitchenObject bowlKitchenObject))
            {
                // Deliver Bowl
                DeliveryManager.Instance.DeliverRecipe(bowlKitchenObject);
                player.GetKitchenObject().DestroySelf();
            }
        }
    }
}
