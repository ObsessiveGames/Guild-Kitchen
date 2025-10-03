using UnityEngine;

public class KitchenObject : MonoBehaviour
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    private IKitchenObjectParent kitchenObjectParent;

    public KitchenObjectSO GetKitchenObjectSO()
    {
        return kitchenObjectSO;
    }

    public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent)
    {
        if (this.kitchenObjectParent != null)
        {
            this.kitchenObjectParent.ClearKitchenObject();
        }

        this.kitchenObjectParent = kitchenObjectParent;

        if (kitchenObjectParent.HasKitchenObject())
        {
            Debug.LogError("IKitchenObjectParent already has a KitchenObject!");
        }

        kitchenObjectParent.SetKitchenObject(this);

        transform.parent = kitchenObjectParent.GetKitchenObjectFollowTransform();
        transform.localPosition = Vector3.zero;
    }

    public IKitchenObjectParent GetKitchenObjectParent()
    {
        return kitchenObjectParent;
    }

    public void DestroySelf()
    {
        kitchenObjectParent.ClearKitchenObject();
        Destroy(gameObject);
    }

    public bool TryGetPlate(out PlateKitchenObject plateKitchenObject)
    {
        if (this is PlateKitchenObject plate)
        {
            plateKitchenObject = plate;
            return true;
        }
        else
        {
            plateKitchenObject = null;
            return false;
        }
    }

    public bool TryGetPot(out PotKitchenObject potKitchenObject)
    {
        if (this is PotKitchenObject pot)
        {
            potKitchenObject = pot;
            return true;
        }
        else
        {
            potKitchenObject = null;
            return false;
        }
    }

    public bool TryGetBowl(out BowlKitchenObject bowlKitchenObject)
    {
        if (this is BowlKitchenObject bowl)
        {
            bowlKitchenObject = bowl;
            return true;
        }
        else
        {
            bowlKitchenObject = null;
            return false;
        }
    }

    public bool TryGetPaperBag(out PaperBagKitchenObject paperBagKitchenObject)
    {
        if (this is PaperBagKitchenObject paperBag)
        {
            paperBagKitchenObject = paperBag;
            return true;
        }
        else
        {
            paperBagKitchenObject = null;
            return false;
        }
    }

    public static KitchenObject SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
    {
        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);
        KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
        kitchenObject.SetKitchenObjectParent(kitchenObjectParent);

        return kitchenObject;
    }
}
