using UnityEngine;

public class CauldronCounter_Visual : MonoBehaviour
{
    [SerializeField] private CauldronCounter cauldronCounter;

    [Header("Visuals")]
    [SerializeField] private GameObject cauldronOn;   // Particle effect (bubbles/steam)
    [SerializeField] private Light cauldronLight;     // Light to glow when cooking

    private void Start()
    {
        cauldronCounter.OnStateChanged += CauldronCounter_OnStateChanged;

        // Initialize visuals to match current state
        SetVisualState(cauldronCounter.GetCurrentState());
    }

    private void CauldronCounter_OnStateChanged(object sender, CauldronCounter.OnStateChangedEventArgs e)
    {
        SetVisualState(e.state);
    }

    private void SetVisualState(CauldronCounter.State state)
    {
        bool isActive = state == CauldronCounter.State.Boiling || state == CauldronCounter.State.Boiled;

        cauldronOn.SetActive(isActive);
        cauldronLight.enabled = isActive;
    }
}
