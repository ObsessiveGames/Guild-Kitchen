using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OvenCounterVisual : MonoBehaviour
{
    [SerializeField] private OvenCounter ovenCounter;
    [SerializeField] private GameObject ovenOnGameObject;
    [SerializeField] private GameObject particlesGameObject;

    private void Start()
    {
        ovenCounter.OnStateChanged += OvenCounter_OnStateChanged;
    }

    private void OvenCounter_OnStateChanged(object sender, OvenCounter.OnStateChangedEventArgs e)
    {
        // Show visual while baking or baked (until overbaked)
        bool showVisual = e.state == OvenCounter.State.Baking || e.state == OvenCounter.State.Baked;
        ovenOnGameObject.SetActive(showVisual);
        particlesGameObject.SetActive(showVisual);
    }
}
