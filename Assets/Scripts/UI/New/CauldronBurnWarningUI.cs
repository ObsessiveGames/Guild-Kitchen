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

        // Only show warning when cauldron is finished boiling but not overboiled yet
        bool show = cauldronCounter.GetCurrentState() == CauldronCounter.State.Finished &&
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
