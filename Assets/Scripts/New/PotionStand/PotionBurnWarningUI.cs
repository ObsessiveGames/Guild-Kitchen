using UnityEngine;

public class PotionBurnWarningUI : MonoBehaviour
{
    [SerializeField] private PotionCounter potionCounter;

    private void Start()
    {
        potionCounter.OnProgressChanged += PotionCounter_OnProgressChanged;
        Hide();
    }

    private void PotionCounter_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        float burnShowProgressAmount = 0.5f;
        bool show = potionCounter.IsBrewed() && e.progressNormalized >= burnShowProgressAmount;

        if (show) Show();
        else Hide();
    }

    private void Show() => gameObject.SetActive(true);
    private void Hide() => gameObject.SetActive(false);
}
