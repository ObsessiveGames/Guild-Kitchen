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
        Finished,
        Overboiled,
    }

    // grabs pots ingredient info
    [SerializeField] private PotKitchenObject currentPot;
    [SerializeField] private PotCompleteVisual potCompleteVisual;

    [SerializeField] private float potBoilTimeMax = 5f;
    [SerializeField] private float potOverboilTimeMax = 4f;


    private State state;
    private float potTimer;

    public State GetCurrentState() => state;

    private void Update()
    {
        if (!currentPot) return;

        switch (state)
        {
            case State.Idle:
                break;

            case State.Boiling:
                potTimer += Time.deltaTime;
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalized = potTimer / potBoilTimeMax
                });

                if (potTimer >= potBoilTimeMax)
                {
                    state = State.Finished;
                    potTimer = 0f;
                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });

                    // Automatically cook the pot ingredients when finished
                    CookPotIngredients();
                }
                break;

            case State.Finished:
                potTimer += Time.deltaTime;
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalized = potTimer / potOverboilTimeMax
                });

                if (potTimer >= potOverboilTimeMax)
                {
                    state = State.Overboiled;
                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = 0f });
                }
                break;

            case State.Overboiled:
                // nothing to do
                break;
        }
    }

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject())
            {
                // Only accept PotKitchenObject
                if (player.GetKitchenObject().TryGetPot(out PotKitchenObject potKitchenObject))
                {
                    currentPot = potKitchenObject; // <--- assign it
                    potKitchenObject.SetKitchenObjectParent(this);
                    state = State.Boiling;
                    potTimer = 0f;

                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = 0f });
                }
            }
        }
        else
        {
            // Player picks up pot
            if (!player.HasKitchenObject())
            {
                GetKitchenObject().SetKitchenObjectParent(player);
                state = State.Idle;
                potTimer = 0f;

                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = 0f });
 
            }
        }
    }

    private void CookPotIngredients()
    {
        if (currentPot == null) return;

        var ingredients = currentPot.GetKitchenObjectSOList();
        currentPot.CookIngredients();

    }

    public bool IsBoilingFinished()
    {
        return potTimer >= potBoilTimeMax; // assuming you have potBoilTimeMax
    }

    public bool IsBoiling() => state == State.Boiling;
    public bool IsFinished() => state == State.Finished;
}
