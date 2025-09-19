using UnityEngine;

public class CauldronCounterSound : MonoBehaviour
{
    [SerializeField] private CauldronCounter cauldronCounter;
    [SerializeField] private AudioClipRefsSO audioClipRefsSO;

    private AudioSource audioSource;
    private float warningSoundTimer;
    private bool playWarningSound;

    private void Awake()
    {
        // AudioSource must be attached to the same GameObject as this script
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true; // Loop the pot cooking sound
        audioSource.clip = audioClipRefsSO.potCooking; // Assign the cooking clip
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

        // Show warning only if the pot is finished boiling and progress is halfway to overboil
        playWarningSound = cauldronCounter.IsFinished() && e.progressNormalized >= burnShowProgressAmount;
    }

    private void CauldronCounter_OnStateChanged(object sender, CauldronCounter.OnStateChangedEventArgs e)
    {
        // Play cooking sound while Boiling or Finished
        bool playSound = e.state == CauldronCounter.State.Boiling || e.state == CauldronCounter.State.Finished;

        if (playSound)
        {
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            if (audioSource.isPlaying)
                audioSource.Pause();
        }
    }
}
