//using System;
//using System.Collections.Generic;
//using UnityEngine;

//public class MixerCounter : BaseCounter, IHasProgress {
//    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
//    public event EventHandler OnMixStarted;
//    public event EventHandler OnMixStopped;
//    public event EventHandler OnMixed;

//    [Header("Single-Item Recipes (optional)")]
//    [SerializeField] private MixingRecipeSO[] singleRecipes;

//    [Header("Multi-Item Recipes (Bowl)")]
//    [SerializeField] private MultiMixRecipeSO[] multiRecipes;

//    [Header("Mode")]
//    [SerializeField] private bool clickToMix = true;
//    [SerializeField] private bool requirePowerOn = false;

//    [Header("FX")]
//    [SerializeField] private AudioSource sfxLoop;
//    [SerializeField] private Animator animator;
//    [SerializeField] private string animBool_IsMixing = "IsMixing";

//    // State
//    private int mixProgress;
//    private float timeLeft;

//    private MixingRecipeSO activeSingle;
//    private MultiMixRecipeSO activeMulti;
//    private bool isMixing;

//    private void Update() {
//        if (!isMixing || clickToMix) return;
//        if (activeSingle == null && activeMulti == null) return;

//        timeLeft -= Time.deltaTime;
//        float total = Mathf.Max(0.01f, (activeSingle != null ? activeSingle.mixSeconds : activeMulti.mixSeconds));
//        float norm = Mathf.Clamp01(1f - (timeLeft / total));
//        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = norm });

//        if (timeLeft <= 0f) {
//            FinishMix();
//        }
//    }
//    public override void Interact(Player player)
//    {
//        // Nothing on counter
//        if (!HasKitchenObject())
//        {
//            if (player.HasKitchenObject())
//            {
//                var ko = player.GetKitchenObject();

//                // If player brings a Bowl, place it here.
//                if (ko.TryGetComponent(out BowlKitchenObject _))
//                {
//                    ko.SetKitchenObjectParent(this);
//                    StopMixingFX();
//                    RecomputeActiveRecipe(); // might auto-detect a multi recipe
//                    return;
//                }

//                // If player brings a loose ingredient and wants to start single mixing:
//                if (TryGetSingleRecipe(ko.GetKitchenObjectSO(), out var sRecipe))
//                {
//                    ko.SetKitchenObjectParent(this);
//                    BeginSingle(sRecipe);
//                    return;
//                }

//                // Otherwise just place it like a clear counter (optional policy)
//                ko.SetKitchenObjectParent(this);
//                StopMixingFX();
//            }
//            return;
//        }

//        // Something is on counter
//        var counterKO = GetKitchenObject();

//        // If player empty-handed: pick up the item/bowl
//        if (!player.HasKitchenObject())
//        {
//            counterKO.SetKitchenObjectParent(player);
//            StopMixingFX();
//            return;
//        }

//        // Player holds a Plate: try pour if we have a bowl with a finished result
//        if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plate))
//        {
//            if (counterKO.TryGetComponent(out BowlKitchenObject bowl))
//            {
//                if (bowl.TryPourOnto(plate))
//                {
//                    StopMixingFX(); // bowl is now empty
//                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = 0f });
//                }
//                return;
//            }

//            // If not a bowl but we have a single mixed result ingredient:
//            if (plate.TryAddIngredient(counterKO.GetKitchenObjectSO()))
//            {
//                counterKO.DestroySelf();
//                StopMixingFX();
//            }
//            return;
//        }

//        // Player holds a loose ingredient and counter has a Bowl: try add to bowl
//        if (counterKO.TryGetComponent(out BowlKitchenObject bowlKO))
//        {
//            var pko = player.GetKitchenObject();
//            if (!pko.TryGetPlate(out _) && !pko.TryGetComponent(out BowlKitchenObject _))
//            {
//                // loose ingredient
//                if (bowlKO.TryAddIngredient(pko.GetKitchenObjectSO()))
//                {
//                    pko.DestroySelf();
//                    StopMixingFX(); // content changed; reset progress
//                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = 0f });
//                    RecomputeActiveRecipe(); // check if we can mix now
//                }
//            }
//            return;
//        }

//        // Otherwise: do nothing special
//    }

//    public override void InteractAlternate(Player player)
//    {
//        // Only click-based mixing uses AltInteract
//        if (!clickToMix) return;
//        if (!HasKitchenObject()) return;
//        if (activeSingle == null && activeMulti == null) return;

//        mixProgress = Mathf.Clamp(mixProgress + 1, 0,
//            activeSingle != null ? activeSingle.mixProgressMax : activeMulti.mixProgressMax);

//        float norm = (activeSingle != null)
//            ? (float)mixProgress / activeSingle.mixProgressMax
//            : (float)mixProgress / activeMulti.mixProgressMax;

//        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = norm });

//        if (!isMixing) StartMixingFX();

//        if ((activeSingle != null && mixProgress >= activeSingle.mixProgressMax) ||
//            (activeMulti != null && mixProgress >= activeMulti.mixProgressMax))
//        {
//            FinishMix();
//        }
//    }

//    // ---------- Internals ----------

//    private void BeginSingle(MixingRecipeSO recipe)
//    {
//        activeSingle = recipe;
//        activeMulti = null;
//        mixProgress = 0;

//        if (clickToMix)
//        {
//            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = 0f });
//            StartMixingFX();
//        }
//        else
//        {
//            timeLeft = Mathf.Max(0.01f, recipe.mixSeconds);
//            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = 0f });
//            if (!requirePowerOn || IsPowered())
//            {
//                isMixing = true;
//                StartMixingFX();
//            }
//        }
//    }

//    private void BeginMulti(MultiMixRecipeSO recipe)
//    {
//        activeSingle = null;
//        activeMulti = recipe;
//        mixProgress = 0;

//        if (clickToMix)
//        {
//            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = 0f });
//            StartMixingFX();
//        }
//        else
//        {
//            timeLeft = Mathf.Max(0.01f, recipe.mixSeconds);
//            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = 0f });
//            if (!requirePowerOn || IsPowered())
//            {
//                isMixing = true;
//                StartMixingFX();
//            }
//        }
//    }

//    private void FinishMix()
//    {
//        // Single-path: replace input with output KO
//        if (activeSingle != null)
//        {
//            var inputKO = GetKitchenObject();
//            var outSO = activeSingle.output;
//            inputKO.DestroySelf();
//            KitchenObject.SpawnKitchenObject(outSO, this);
//        }
//        // Multi-path: mutate the bowl contents to a single result ingredient
//        else if (activeMulti != null && GetKitchenObject().TryGetComponent(out BowlKitchenObject bowl))
//        {
//            var outSO = activeMulti.output;
//            bowl.SetMixedResult(outSO);
//        }

//        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = 1f });
//        OnMixed?.Invoke(this, EventArgs.Empty);
//        StopMixingFX();

//        // Reset so next batch can start when contents change again
//        activeSingle = null;
//        activeMulti = null;
//        mixProgress = 0;
//    }

//    private void StartMixingFX()
//    {
//        if (isMixing) return;
//        isMixing = true;
//        OnMixStarted?.Invoke(this, EventArgs.Empty);
//        if (animator) animator.SetBool(animBool_IsMixing, true);
//        if (sfxLoop && !sfxLoop.isPlaying) sfxLoop.Play();
//    }

//    private void StopMixingFX()
//    {
//        if (!isMixing) return;
//        isMixing = false;
//        OnMixStopped?.Invoke(this, EventArgs.Empty);
//        if (animator) animator.SetBool(animBool_IsMixing, false);
//        if (sfxLoop && sfxLoop.isPlaying) sfxLoop.Stop();
//    }

//    private bool IsPowered() => true;

//    private bool TryGetSingleRecipe(KitchenObjectSO input, out MixingRecipeSO found)
//    {
//        foreach (var r in singleRecipes)
//        {
//            if (r != null && r.input == input) { found = r; return true; }
//        }
//        found = null;
//        return false;
//    }

//    /// <summary>
//    /// Checks the item currently on the counter; if it's a bowl,
//    /// tries to match multi-ingredient recipes. If match found, primes mixer.
//    /// </summary>
//    private void RecomputeActiveRecipe()
//    {
//        if (!HasKitchenObject()) { activeMulti = null; return; }
//        if (!GetKitchenObject().TryGetComponent(out BowlKitchenObject bowl)) { activeMulti = null; return; }

//        var current = bowl.BuildCounts();
//        foreach (var r in multiRecipes)
//        {
//            if (r == null) continue;
//            if (MultisetEquals(current, r.BuildRequiredCounts()))
//            {
//                // Auto-begin only in time-based mode (so player doesn’t need to click),
//                // for click mode we just arm the recipe; player presses Alt to advance.
//                if (clickToMix)
//                {
//                    BeginMulti(r);
//                    StopMixingFX(); // wait for first click to start FX
//                }
//                else
//                {
//                    BeginMulti(r);
//                }
//                return;
//            }
//        }
//        // No match
//        activeMulti = null;
//        StopMixingFX();
//        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = 0f });
//    }

//    private static bool MultisetEquals(Dictionary<KitchenObjectSO, int> have, Dictionary<KitchenObjectSO, int> need)
//    {
//        if (have.Count != need.Count) return false;
//        foreach (var kv in need)
//        {
//            if (!have.TryGetValue(kv.Key, out var got)) return false;
//            if (got != kv.Value) return false;
//        }
//        return true;
//    }
//}