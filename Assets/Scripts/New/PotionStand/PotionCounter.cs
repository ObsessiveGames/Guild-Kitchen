using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionCounter : BaseCounter, IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public event EventHandler OnPotionAdded;

    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }

    public enum State
    {
        Idle,
        Brewing,
        Brewed,
        Overbrewed,
    }

    [SerializeField] private PotionRecipeSO[] potionRecipeSOArray;
    [SerializeField] private OverbrewRecipeSO[] overbrewRecipeSOArray;

    private State state;
    private float brewingTimer;
    private float overbrewTimer;
    private PotionRecipeSO potionRecipeSO;
    private OverbrewRecipeSO overbrewRecipeSO;

    private void Start()
    {
        state = State.Idle;
    }

    private void Update()
    {
        if (HasKitchenObject())
        {
            switch (state)
            {
                case State.Idle:
                    break;
                case State.Brewing:
                    brewingTimer += Time.deltaTime;

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = brewingTimer / potionRecipeSO.brewingTimeMax
                    });

                    if (brewingTimer > potionRecipeSO.brewingTimeMax)
                    {
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(potionRecipeSO.output, this);

                        state = State.Brewed;
                        overbrewTimer = 0f;
                        overbrewRecipeSO = GetOverbrewRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
                        OnPotionAdded?.Invoke(this, EventArgs.Empty); // Fire event here
                    }
                    break;

                case State.Brewed:
                    overbrewTimer += Time.deltaTime;

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = overbrewTimer / overbrewRecipeSO.overbrewTimeMax
                    });

                    if (overbrewTimer > overbrewRecipeSO.overbrewTimeMax)
                    {
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(overbrewRecipeSO.output, this);

                        state = State.Overbrewed;

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = 0f });
                    }
                    break;

                case State.Overbrewed:
                    break;
            }
        }
    }

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject())
            {
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    potionRecipeSO = GetPotionRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                    state = State.Brewing;
                    brewingTimer = 0f;

                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = brewingTimer / potionRecipeSO.brewingTimeMax
                    });
                }
            }
        }
        else
        {
            if (player.HasKitchenObject())
            {
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        GetKitchenObject().DestroySelf();
                        state = State.Idle;

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = 0f });
                    }
                }
            }
            else
            {
                GetKitchenObject().SetKitchenObjectParent(player);
                state = State.Idle;

                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = 0f });
            }
        }
    }

    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        return GetPotionRecipeSOWithInput(inputKitchenObjectSO) != null;
    }

    private PotionRecipeSO GetPotionRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (PotionRecipeSO recipe in potionRecipeSOArray)
        {
            if (recipe.input == inputKitchenObjectSO) return recipe;
        }
        return null;
    }

    private OverbrewRecipeSO GetOverbrewRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (OverbrewRecipeSO recipe in overbrewRecipeSOArray)
        {
            if (recipe.input == inputKitchenObjectSO) return recipe;
        }
        return null;
    }

    public bool IsBrewed()
    {
        return state == State.Brewed;
    }
}
