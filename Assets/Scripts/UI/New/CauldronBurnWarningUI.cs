using UnityEngine;

public class CauldronBurnWarningUI : MonoBehaviour
{
    [SerializeField] private CauldronCounter cauldronCounter;

    private void Start()
    {
        cauldronCounter.OnProgressChanged += CauldronCounter_OnProgressChanged;
        Hide();
    }

    private void CauldronCounter_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        float burnShowProgressAmount = 0.5f;

        // Show warning if pot is finished boiling AND progress is past halfway to overboil
        bool show = cauldronCounter.IsBoilingFinished() &&
                    e.progressNormalized >= burnShowProgressAmount;

        if (show) Show();
        else Hide();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
