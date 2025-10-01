using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionCounter : BaseCounter, IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged; // used by progress bars
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public event EventHandler OnPotionAdded; // fired after a succefull potion is brewed

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
        IncorrectIngredient
    }

    [Header("Transforms")]
    [SerializeField] private Transform ingredientPlacementPoint;       // Where ingredients appear
    [SerializeField] private Transform potionPlacementPoint;   // Where final potion appears

    [Header("Recipes")]
    [SerializeField] private PotionRecipeSO[] potionRecipeSOArray;
    [SerializeField] private OverbrewRecipeSO[] overbrewRecipeSOArray;

    [Header("Animator & Effects")]
    [SerializeField] private Animator animator;
    [SerializeField] private float failedCookTime = 1f;
    [SerializeField] private Onomatopoeia onomatopoeia;
    [SerializeField] private AudioSource failedCookAudio;


    private List<KitchenObject> currentIngredients = new List<KitchenObject>();
    private KitchenObject currentPotion;

    private State state = State.Idle;
    private float brewingTimer;
    private float overbrewTimer;
    private float failedCookTimer;
    private PotionRecipeSO potionRecipeSO;
    private OverbrewRecipeSO overbrewRecipeSO;

    private void Update()
    {
        switch (state)
        {
            case State.Brewing:
                brewingTimer += Time.deltaTime;

                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalized = brewingTimer / potionRecipeSO.brewingTimeMax
                });

                if (brewingTimer >= potionRecipeSO.brewingTimeMax)
                    FinishBrewing();
                break;

            case State.Brewed:
                if (currentPotion == null) break;

                overbrewTimer += Time.deltaTime;

                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalized = overbrewTimer / overbrewRecipeSO.overbrewTimeMax
                });

                if (overbrewTimer >= overbrewRecipeSO.overbrewTimeMax)
                    OverbrewPotion();
                break;

            case State.IncorrectIngredient:
                failedCookTimer -= Time.deltaTime;

                // Report progress to progress bar
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalized = 1f - (failedCookTimer / failedCookTime)
                });

                if (failedCookTimer <= 0f)
                {
                    // BOOM animation
                    if (onomatopoeia != null)
                        onomatopoeia.Boom();

                    // Stop audio
                    if (failedCookAudio != null && failedCookAudio.isPlaying)
                        failedCookAudio.Stop();

                    ClearIngredients();
                    state = State.Idle;

                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = 0f });
                }

                break;

        }
    }

    public override void Interact(Player player)
    {
        if (player.HasKitchenObject())
        {
            KitchenObject heldObject = player.GetKitchenObject();

            if (state == State.Idle)
            {
                AddIngredient(heldObject, player); // Now safe: counter is idle
                DebugIngredients();
            }
            else
            {
                // Counter is busy: do NOT clear player
                Debug.Log("Counter is busy! Wait until brewing finishes.");
            }
        }

        else
        {
            // Player picks up potion if available
            if (currentPotion != null && (state == State.Brewed || state == State.Overbrewed))
            {
                currentPotion.SetKitchenObjectParent(player);
                currentPotion = null;
                state = State.Idle;

                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = 0f });
            }
        }
    }


    public override void InteractAlternate(Player player)
    {
        if (currentIngredients.Count > 0 && state == State.Idle)
        {
            potionRecipeSO = GetPotionRecipeSOWithIngredients(currentIngredients);

            if (potionRecipeSO != null)
            {
                state = State.Brewing;
                brewingTimer = 0f;

                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = 0f });
            }
            else
            {
                state = State.IncorrectIngredient;
                failedCookTimer = failedCookTime;

                if (failedCookAudio != null && !failedCookAudio.isPlaying)
                    failedCookAudio.Play();

                if (animator != null)
                    animator.SetTrigger("FailedCook");

                Debug.Log("Incorrect ingredients! Triggering failed cook.");
            }
        }
    }

    private void FinishBrewing()
    {
        // Destroy ingredients
        foreach (KitchenObject ingredient in currentIngredients)
            ingredient.DestroySelf();
        currentIngredients.Clear();

        // Spawn potion at potionPlacementPoint
        currentPotion = KitchenObject.SpawnKitchenObject(potionRecipeSO.output, this);
        currentPotion.transform.position = potionPlacementPoint.position;
        currentPotion.transform.rotation = potionPlacementPoint.rotation;

        state = State.Brewed;
        overbrewTimer = 0f;
        overbrewRecipeSO = GetOverbrewRecipeSOWithInput(potionRecipeSO.output);

        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
        OnPotionAdded?.Invoke(this, EventArgs.Empty);
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = 0f });
    }

    private void OverbrewPotion()
    {
        if (currentPotion != null)
            currentPotion.DestroySelf();

        currentPotion = KitchenObject.SpawnKitchenObject(overbrewRecipeSO.output, this);
        currentPotion.transform.position = potionPlacementPoint.position;
        currentPotion.transform.rotation = potionPlacementPoint.rotation;

        state = State.Overbrewed;
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = 0f });

        // Optional: Add BOOM VFX or sound here
    }

    private PotionRecipeSO GetPotionRecipeSOWithIngredients(List<KitchenObject> ingredients)
    {
        foreach (PotionRecipeSO recipe in potionRecipeSOArray)
        {
            if (recipe.inputList.Length != ingredients.Count) continue;

            bool allMatch = true;
            foreach (KitchenObjectSO requiredSO in recipe.inputList)
            {
                bool found = false;
                foreach (KitchenObject ingredient in ingredients)
                {
                    if (ingredient.GetKitchenObjectSO() == requiredSO)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    allMatch = false;
                    break;
                }
            }

            if (allMatch) return recipe;
        }
        return null;
    }

    private OverbrewRecipeSO GetOverbrewRecipeSOWithInput(KitchenObjectSO input)
    {
        foreach (var recipe in overbrewRecipeSOArray)
        {
            if (recipe.input == input) return recipe;
        }
        return null;
    }

    public bool IsBrewed() => state == State.Brewed;

    private void DebugIngredients()
    {
        string debugStr = "Ingredients on counter: ";
        foreach (var ingredient in currentIngredients)
        {
            debugStr += ingredient.GetKitchenObjectSO().name + ", ";
        }
        Debug.Log(debugStr);
    }

    // --- Private helper to safely add ingredients ---
    private void AddIngredient(KitchenObject ingredient, Player player)
    {
        if (ingredient == null) return;

        // Remove from player
        if (ingredient.GetKitchenObjectParent() == player)
        {
            player.ClearKitchenObject();
        }

        // Place ingredient on counterTopPoint
        ingredient.transform.parent = ingredientPlacementPoint;
        ingredient.transform.localPosition = Vector3.zero;
        ingredient.transform.localRotation = Quaternion.identity;

        // Add to ingredient list
        currentIngredients.Add(ingredient);
    }

    private void ClearIngredients()
    {
        foreach (var ingredient in currentIngredients)
        {
            if (ingredient != null)
                ingredient.DestroySelf();
        }
        currentIngredients.Clear();
    }
}
