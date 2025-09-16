using System;
using UnityEngine;

public class CauldronCounter_Visual : MonoBehaviour
{

    [SerializeField] private CauldronCounter cauldronCounter;
    [SerializeField] private GameObject boilingParticlesGameObject;
    [SerializeField] private GameObject overboilParticlesGameObject;
    [SerializeField] private Animator animator;

    private const string IS_BOILING = "IsBoiling";

    private void Start()
    {
        cauldronCounter.OnStateChanged += CauldronCounter_OnStateChanged;

        // Set initial state
        SetVisualState(cauldronCounter.IsBoiled() ? CauldronCounter.State.Boiled : CauldronCounter.State.Idle);
    }

    private void CauldronCounter_OnStateChanged(object sender, CauldronCounter.OnStateChangedEventArgs e)
    {
        SetVisualState(e.state);
    }

    private void SetVisualState(CauldronCounter.State state)
    {
        switch (state)
        {
            case CauldronCounter.State.Idle:
                boilingParticlesGameObject.SetActive(false);
                overboilParticlesGameObject.SetActive(false);
                animator.SetBool(IS_BOILING, false);
                break;

            case CauldronCounter.State.Boiling:
                boilingParticlesGameObject.SetActive(true);
                overboilParticlesGameObject.SetActive(false);
                animator.SetBool(IS_BOILING, true);
                break;

            case CauldronCounter.State.Boiled:
                boilingParticlesGameObject.SetActive(false);
                overboilParticlesGameObject.SetActive(true); // Steam rising or warning
                animator.SetBool(IS_BOILING, false);
                break;

            case CauldronCounter.State.Burned:
                boilingParticlesGameObject.SetActive(false);
                overboilParticlesGameObject.SetActive(false);
                animator.SetBool(IS_BOILING, false);
                break;
        }
    }
}
