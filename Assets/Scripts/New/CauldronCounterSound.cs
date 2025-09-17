using UnityEngine;

public class CauldronCounterSound : MonoBehaviour
{
    [SerializeField] private CauldronCounter cauldronCounter;

    private AudioSource audioSource;
    private float warningSoundTimer;
    private bool playWarningSound;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        cauldronCounter.OnStateChanged += CauldronCounter_OnStateChanged;
        cauldronCounter.OnProgressChanged += CauldronCounter_OnProgressChanged;
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

                // Play the warning sound at the cauldron's position
                SoundManager.Instance.PlayWarningSound(cauldronCounter.transform.position);
            }
        }
    }

    private void CauldronCounter_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        float burnShowProgressAmount = 0.5f;
        // Only play warning if food is boiled and overboiling progress is above halfway
        playWarningSound = cauldronCounter.GetCurrentState() == CauldronCounter.State.Boiled &&
                           e.progressNormalized >= burnShowProgressAmount;
    }

    private void CauldronCounter_OnStateChanged(object sender, CauldronCounter.OnStateChangedEventArgs e)
    {
        // Play looping cooking sound while boiling or boiled
        bool playSound = e.state == CauldronCounter.State.Boiling || e.state == CauldronCounter.State.Boiled;

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
