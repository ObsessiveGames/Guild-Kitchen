using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleClearCounter : BaseCounter
{
    public event EventHandler OnPlayerGrabbedObject;

    [Header("Candle Settings")]
    [SerializeField] private KitchenObjectSO candleKitchenObjectSO;
    [SerializeField] private GameObject candleVisual;  // Reference to the candle visual GameObject on the counter
    [SerializeField] private int maxCandles = 2;

    private int candlesGiven = 0;
    private KitchenObject spawnedCandleKitchenObject;

    private void Start()
    {
        // Make sure candle visual is active at start
        if (candleVisual != null)
            candleVisual.SetActive(true);

        // Spawn candle KitchenObject on counter (linked to visual)
        SpawnCandle();
    }

    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject() && HasKitchenObject())
        {
            // Player empty-handed, counter has an object (likely a candle)
            KitchenObject kitchenObjectOnCounter = GetKitchenObject();
            if (kitchenObjectOnCounter != null)
            {
                kitchenObjectOnCounter.SetKitchenObjectParent(player);

                candlesGiven++;

                OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);

                if (candleVisual != null)
                    candleVisual.SetActive(false);

                ClearKitchenObject();
                spawnedCandleKitchenObject = null;

                if (candlesGiven < maxCandles)
                {
                    StartCoroutine(RespawnCandleAfterDelay(1f));
                }
            }
            return;
        }

        if (HasKitchenObject())
        {
            KitchenObject kitchenObjectOnCounter = GetKitchenObject();

            if (player.HasKitchenObject())
            {
                // Player is carrying something
                KitchenObject playerKitchenObject = player.GetKitchenObject();

                // --- PLAYER HOLDS PLATE ---
                if (playerKitchenObject.TryGetPlate(out PlateKitchenObject plateKitchenObject))
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
                if (playerKitchenObject.TryGetPot(out PotKitchenObject potKitchenObject))
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
                if (playerKitchenObject.TryGetBowl(out BowlKitchenObject bowlKitchenObject))
                {
                    // Bowl (in hand) <- Pot (on counter)
                    if (kitchenObjectOnCounter.TryGetPot(out PotKitchenObject potOnCounter))
                    {
                        if (potOnCounter.GetKitchenObjectSOList().Count > 0)
                        {
                            bowlKitchenObject.CopyIngredientsFromPot(potOnCounter);
                            potOnCounter.ClearKitchenObjects();

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
                    if (playerKitchenObject != null && playerKitchenObject.GetKitchenObjectSO() != null)
                    {
                        if (plateOnCounter.TryAddIngredient(playerKitchenObject.GetKitchenObjectSO()))
                        {
                            playerKitchenObject.DestroySelf();
                        }
                    }
                }
                else if (kitchenObjectOnCounter.TryGetPot(out PotKitchenObject potOnCounter2))
                {
                    if (playerKitchenObject != null && playerKitchenObject.GetKitchenObjectSO() != null)
                    {
                        if (potOnCounter2.TryAddIngredient(playerKitchenObject.GetKitchenObjectSO()))
                        {
                            playerKitchenObject.DestroySelf();
                        }
                    }
                }
            }
            else
            {
                // Player empty-handed, counter has object - player picks it up
                kitchenObjectOnCounter.SetKitchenObjectParent(player);
            }
        }
        else
        {
            // Counter empty, player carrying something - place on counter
            if (player.HasKitchenObject())
            {
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
        }
    }

    private IEnumerator RespawnCandleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (candlesGiven < maxCandles)
        {
            // Enable the candle visual again
            if (candleVisual != null)
                candleVisual.SetActive(true);

            // Spawn candle KitchenObject again and assign it
            SpawnCandle();
        }
    }

    private void SpawnCandle()
    {
        if (spawnedCandleKitchenObject == null)
        {
            spawnedCandleKitchenObject = KitchenObject.SpawnKitchenObject(candleKitchenObjectSO, this);
        }
    }
}
