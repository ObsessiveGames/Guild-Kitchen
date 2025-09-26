using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionCounterVisual : MonoBehaviour
{
    [SerializeField] private PotionCounter potionCounter;
    [SerializeField] private GameObject potionOnGameObject;
    [SerializeField] private GameObject particlesGameObject;

    private void Start()
    {
        potionCounter.OnStateChanged += PotionCounter_OnStateChanged;
    }

    private void PotionCounter_OnStateChanged(object sender, PotionCounter.OnStateChangedEventArgs e)
    {
        bool showVisual = e.state == PotionCounter.State.Brewing || e.state == PotionCounter.State.Brewed;
        potionOnGameObject.SetActive(showVisual);
        particlesGameObject.SetActive(showVisual);
    }
}
