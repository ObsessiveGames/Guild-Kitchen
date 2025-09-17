using System;
using UnityEngine;

public class CauldronCounter : BaseCounter, IHasProgress
{

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;

    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }

    public enum State
    {
        Idle,
        Boiling,
        Boiled,
        Burned,
    }

    [SerializeField] private BoilingRecipeSO[] boilingRecipeSOArray;
    [SerializeField] private OverboilRecipeSO[] overboilRecipeSOArray;

    private State state;
    private float boilingTimer;
    private float overboilTimer;

    private BoilingRecipeSO boilingRecipeSO;
    private OverboilRecipeSO overboilRecipeSO;

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
                case State.Boiling:
                    boilingTimer += Time.deltaTime;

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = boilingTimer / boilingRecipeSO.boilingTimeMax
                    });

                    if (boilingTimer > boilingRecipeSO.boilingTimeMax)
                    {
                        // Done Boiling
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(boilingRecipeSO.output, this);

                        state = State.Boiled;
                        overboilTimer = 0f;
                        overboilRecipeSO = GetOverboilRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
                    }
                    break;

                case State.Boiled:
                    overboilTimer += Time.deltaTime;

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = overboilTimer / overboilRecipeSO.overboilTimeMax
                    });

                    if (overboilTimer > overboilRecipeSO.overboilTimeMax)
                    {
                        // Overboiled
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(overboilRecipeSO.output, this);

                        state = State.Burned;

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                        {
                            progressNormalized = 0f
                        });
                    }
                    break;

                case State.Burned:
                    break;
            }
        }
    }

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            // Cauldron is empty
            if (player.HasKitchenObject())
            {
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    // Start Boiling
                    player.GetKitchenObject().SetKitchenObjectParent(this);

                    boilingRecipeSO = GetBoilingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                    state = State.Boiling;
                    boilingTimer = 0f;

                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = 0f
                    });
                }
            }
        }
        else
        {
            // Cauldron has something
            if (player.HasKitchenObject())
            {
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        GetKitchenObject().DestroySelf();
                        state = State.Idle;

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                        {
                            progressNormalized = 0f
                        });
                    }
                }
            }
            else
            {
                // Player takes cooked object
                GetKitchenObject().SetKitchenObjectParent(player);
                state = State.Idle;

                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalized = 0f
                });
            }
        }
    }

    private bool HasRecipeWithInput(KitchenObjectSO input)
    {
        return GetBoilingRecipeSOWithInput(input) != null;
    }

    private BoilingRecipeSO GetBoilingRecipeSOWithInput(KitchenObjectSO input)
    {
        foreach (BoilingRecipeSO recipe in boilingRecipeSOArray)
        {
            if (recipe.input == input) return recipe;
        }
        return null;
    }

    private OverboilRecipeSO GetOverboilRecipeSOWithInput(KitchenObjectSO input)
    {
        foreach (OverboilRecipeSO recipe in overboilRecipeSOArray)
        {
            if (recipe.input == input) return recipe;
        }
        return null;
    }

    public bool IsBoiled()
    {
        return state == State.Boiled;
    }

    public State GetCurrentState()
    {
        return state;
    }

}
