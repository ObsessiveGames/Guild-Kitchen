using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotCounter : BaseCounter
{
    public event EventHandler OnPotSpawned;
    public event EventHandler OnPotRemoved;

    [SerializeField] private KitchenObjectSO potKitchenObjectSO;

    private float spawnPotTimer;
    private float spawnPotTimerMax = 4f;
    private int potsSpawnedAmount;
    private int potsSpawnedAmountMax = 1;

    private void Update()
    {
        spawnPotTimer += Time.deltaTime;
        if (spawnPotTimer > spawnPotTimerMax)
        {
            spawnPotTimer = 0f;

            if (KitchenGameManager.Instance.IsGamePlaying() && potsSpawnedAmount < potsSpawnedAmountMax)
            {
                potsSpawnedAmount++;

                OnPotSpawned?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            // Player is empty handed
            if (potsSpawnedAmount > 0)
            {
                // There's at least one pot here
                potsSpawnedAmount--;

                KitchenObject.SpawnKitchenObject(potKitchenObjectSO, player);

                OnPotRemoved?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
