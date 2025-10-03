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
            KitchenObject kitchenObject = player.GetKitchenObject();

            if (kitchenObject.TryGetPlate(out PlateKitchenObject plateKitchenObject))
            {
                DeliveryManager.Instance.DeliverRecipe(plateKitchenObject);
                kitchenObject.DestroySelf();
            }
            else if (kitchenObject.TryGetBowl(out BowlKitchenObject bowlKitchenObject))
            {
                DeliveryManager.Instance.DeliverRecipe(bowlKitchenObject);
                kitchenObject.DestroySelf();
            }
            else if (kitchenObject.TryGetPaperBag(out PaperBagKitchenObject paperBagKitchenObject))
            {
                DeliveryManager.Instance.DeliverRecipe(paperBagKitchenObject);
                kitchenObject.DestroySelf();
            }
        }
    }
}
