using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OvenCounter : BaseCounter, IHasProgress
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
        Baking,
        Baked,
        Overbaked,
    }

    [SerializeField] private BakingRecipeSO[] bakingRecipeSOArray;
    [SerializeField] private OverbakeRecipeSO[] overbakeRecipeSOArray;

    private State state;
    private float bakingTimer;
    private float overbakeTimer;
    private BakingRecipeSO bakingRecipeSO;
    private OverbakeRecipeSO overbakeRecipeSO;

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
                case State.Baking:
                    bakingTimer += Time.deltaTime;

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = bakingTimer / bakingRecipeSO.bakingTimeMax
                    });

                    if (bakingTimer > bakingRecipeSO.bakingTimeMax)
                    {
                        // Baked
                        GetKitchenObject().DestroySelf();

                        KitchenObject.SpawnKitchenObject(bakingRecipeSO.output, this);

                        state = State.Baked;
                        overbakeTimer = 0f;
                        overbakeRecipeSO = GetOverbakeRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {
                            state = state
                        });
                    }
                    break;
                case State.Baked:
                    overbakeTimer += Time.deltaTime;

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = overbakeTimer / overbakeRecipeSO.overbakeTimeMax
                    });

                    if (overbakeTimer > overbakeRecipeSO.overbakeTimeMax)
                    {
                        // Overbaked
                        GetKitchenObject().DestroySelf();

                        KitchenObject.SpawnKitchenObject(overbakeRecipeSO.output, this);

                        state = State.Overbaked;

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {
                            state = state
                        });

                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                        {
                            progressNormalized = 0f
                        });
                    }
                    break;
                case State.Overbaked:
                    break;
            }
        }
    }

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            // There is no KitchenObject here
            if (player.HasKitchenObject())
            {
                // Player is carrying something
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    // Player carrying something that can be baked
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    bakingRecipeSO = GetBakingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                    state = State.Baking;
                    bakingTimer = 0f;

                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                    {
                        state = state
                    });

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = bakingTimer / bakingRecipeSO.bakingTimeMax
                    });
                }
            }
            else
            {
                // Player not carrying anything
            }
        }
        else
        {
            // There is a KitchenObject here
            if (player.HasKitchenObject())
            {
                // Player is carrying something
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    // Player is holding a Plate
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        GetKitchenObject().DestroySelf();

                        state = State.Idle;

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {
                            state = state
                        });

                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                        {
                            progressNormalized = 0f
                        });
                    }
                }
            }
            else
            {
                // Player is not carrying anything
                GetKitchenObject().SetKitchenObjectParent(player);

                state = State.Idle;

                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                {
                    state = state
                });

                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalized = 0f
                });
            }
        }
    }

    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        BakingRecipeSO bakingRecipeSO = GetBakingRecipeSOWithInput(inputKitchenObjectSO);
        return bakingRecipeSO != null;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        BakingRecipeSO bakingRecipeSO = GetBakingRecipeSOWithInput(inputKitchenObjectSO);
        if (bakingRecipeSO != null)
        {
            return bakingRecipeSO.output;
        }
        else
        {
            return null;
        }
    }

    private BakingRecipeSO GetBakingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (BakingRecipeSO bakingRecipeSO in bakingRecipeSOArray)
        {
            if (bakingRecipeSO.input == inputKitchenObjectSO)
            {
                return bakingRecipeSO;
            }
        }
        return null;
    }

    private OverbakeRecipeSO GetOverbakeRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (OverbakeRecipeSO overbakeRecipeSO in overbakeRecipeSOArray)
        {
            if (overbakeRecipeSO.input == inputKitchenObjectSO)
            {
                return overbakeRecipeSO;
            }
        }
        return null;
    }

    public bool IsBaked()
    {
        return state == State.Baked;
    }
}
