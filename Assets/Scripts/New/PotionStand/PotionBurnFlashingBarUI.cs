using UnityEngine;

public class PotionBurnFlashingBarUI : MonoBehaviour
{
    private const string IS_FLASHING = "IsFlashing";

    [SerializeField] private PotionCounter potionCounter;
    private Animator animator;

    private void Awake() => animator = GetComponent<Animator>();

    private void Start()
    {
        potionCounter.OnProgressChanged += PotionCounter_OnProgressChanged;
        animator.SetBool(IS_FLASHING, false);
    }

    private void PotionCounter_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        float burnShowProgressAmount = 0.5f;
        bool show = potionCounter.IsBrewed() && e.progressNormalized >= burnShowProgressAmount;
        animator.SetBool(IS_FLASHING, show);
    }
}
