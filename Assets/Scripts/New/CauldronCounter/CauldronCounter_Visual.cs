using System.Collections.Generic;
using UnityEngine;

public class CauldronCounter_Visual : MonoBehaviour
{
    [SerializeField] private CauldronCounter cauldronCounter;

    [Header("Visuals")]
    [SerializeField] private List<GameObject> cauldronOn;   // List of visual effects (bubbles, steam, etc.)
    [SerializeField] private Light cauldronLight;           // Light to glow when cooking

    private void Start()
    {
        // Subscribe to state change events
        cauldronCounter.OnStateChanged += CauldronCounter_OnStateChanged;

        // Initialize visuals to match the current cauldron state
        SetVisualState(cauldronCounter.GetCurrentState());
    }

    private void CauldronCounter_OnStateChanged(object sender, CauldronCounter.OnStateChangedEventArgs e)
    {
        SetVisualState(e.state);
    }

    private void SetVisualState(CauldronCounter.State state)
    {
        // Determine if the cauldron is active (Boiling or Finished)
        bool isActive = state == CauldronCounter.State.Boiling || state == CauldronCounter.State.Finished;

        // Enable/disable all visual GameObjects
        foreach (GameObject visual in cauldronOn)
        {
            if (visual != null)
            {
                visual.SetActive(isActive);
            }
        }

        // Enable/disable the cauldron light
        if (cauldronLight != null)
        {
            cauldronLight.enabled = isActive;
        }
    }
}
