using UnityEngine;

public class CauldronBurnFlashingBarUI : MonoBehaviour
{
    private const string IS_FLASHING = "IsFlashing";

    [SerializeField] private CauldronCounter cauldronCounter;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        cauldronCounter.OnProgressChanged += CauldronCounter_OnProgressChanged;

        animator.SetBool(IS_FLASHING, false);
    }

    private void CauldronCounter_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        float burnShowProgressAmount = 0.5f;

        // Show flashing only if the food is Finished (formerly Boiled) and progress is halfway to overboil
        bool show = cauldronCounter.GetCurrentState() == CauldronCounter.State.Finished &&
                    e.progressNormalized >= burnShowProgressAmount;

        animator.SetBool(IS_FLASHING, show);
    }

}
