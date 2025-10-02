using System.Collections;
using UnityEngine;

public class LightingComponent : MonoBehaviour
{
    [Header("Child Object")]
    [SerializeField] private GameObject lightningChild;

    [Header("Other Object to Disable During Lightning")]
    [SerializeField] private GameObject targetToDisable;

    [Header("Audio")]
    [SerializeField] private AudioSource stormAudioSource;      // Constantly playing storm
    [SerializeField] private AudioClip[] lightningClips;        // Array of lightning sounds
    [SerializeField] private AudioSource lightningAudioSource;  // AudioSource for lightning

    [Header("Lightning Settings")]
    [SerializeField] private float lightningInterval = 20f;     // Time between lightning strikes
    [SerializeField] private float minFlashTime = 0.05f;        // Minimum time child is on/off during flash
    [SerializeField] private float maxFlashTime = 0.2f;         // Maximum time child is on/off during flash

    private void Start()
    {
        if (stormAudioSource != null && !stormAudioSource.isPlaying)
            stormAudioSource.Play();

        StartCoroutine(LightningRoutine());
    }

    private IEnumerator LightningRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(lightningInterval);

            // Play a random lightning sound
            if (lightningClips.Length > 0 && lightningAudioSource != null)
            {
                int index = Random.Range(0, lightningClips.Length);
                lightningAudioSource.clip = lightningClips[index];
                lightningAudioSource.Play();
            }

            // Flash child object randomly 1–3 times
            int flashes = Random.Range(1, 4);
            for (int i = 0; i < flashes; i++)
            {
                // Turn on lightningChild and disable target
                lightningChild.SetActive(true);
                if (targetToDisable != null) targetToDisable.SetActive(false);

                yield return new WaitForSeconds(Random.Range(minFlashTime, maxFlashTime));

                // Turn off lightningChild and re-enable target
                lightningChild.SetActive(false);
                if (targetToDisable != null) targetToDisable.SetActive(true);

                yield return new WaitForSeconds(Random.Range(minFlashTime, maxFlashTime));
            }
        }
    }
}
