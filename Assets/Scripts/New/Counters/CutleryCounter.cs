using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutleryCounter : BaseCounter
{
    public event EventHandler OnCutlerySpawned;
    public event EventHandler OnCutleryRemoved;

    [SerializeField] private KitchenObjectSO cutleryKitchenObjectSO;

    private float spawnCutleryTimer;
    private float spawnCutleryTimerMax = 4f;
    private int cutlerySpawnedAmount;
    [SerializeField] private int cutlerySpawnedAmountMax = 4;

    private void Update()
    {
        spawnCutleryTimer += Time.deltaTime;
        if (spawnCutleryTimer > spawnCutleryTimerMax)
        {
            spawnCutleryTimer = 0f;

            if (KitchenGameManager.Instance.IsGamePlaying() && cutlerySpawnedAmount < cutlerySpawnedAmountMax)
            {
                cutlerySpawnedAmount++;

                OnCutlerySpawned?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            // Player is empty handed
            if (cutlerySpawnedAmount > 0)
            {
                // There's at least one cutlrey here
                cutlerySpawnedAmount--;

                KitchenObject.SpawnKitchenObject(cutleryKitchenObjectSO, player);

                OnCutleryRemoved?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
