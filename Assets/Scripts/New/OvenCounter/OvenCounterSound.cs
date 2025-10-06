using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OvenCounterSound : MonoBehaviour
{
    [SerializeField] private OvenCounter ovenCounter;

    private AudioSource audioSource;
    private float warningSoundTimer;
    private bool playWarningSound;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (ovenCounter == null)
        {
            Debug.LogError("OvenCounter reference is missing!");
            return;
        }

        ovenCounter.OnStateChanged += OvenCounter_OnStateChanged;
        ovenCounter.OnProgressChanged += OvenCounter_OnProgressChanged;
    }

    private void Update()
    {
        if (playWarningSound)
        {
            warningSoundTimer -= Time.deltaTime;
            if (warningSoundTimer <= 0f)
            {
                float warningSoundTimerMax = 0.2f;
                warningSoundTimer = warningSoundTimerMax;

                SoundManager.Instance.PlayWarningSound(ovenCounter.transform.position);
            }
        }
    }

    private void OvenCounter_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        float burnShowProgressAmount = 0.5f;
        playWarningSound = ovenCounter.IsBaked() && e.progressNormalized >= burnShowProgressAmount;
    }

    private void OvenCounter_OnStateChanged(object sender, OvenCounter.OnStateChangedEventArgs e)
    {
        bool playSound = e.state == OvenCounter.State.Baking || e.state == OvenCounter.State.Baked;
        if (playSound)
        {
            audioSource.Play();
        }
        else
        {
            audioSource.Pause();
        }
    }
}
