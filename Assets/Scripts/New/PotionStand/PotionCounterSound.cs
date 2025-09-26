using UnityEngine;

public class PotionCounterSound : MonoBehaviour
{
    [SerializeField] private PotionCounter potionCounter;
    [SerializeField] private AudioClip potionAddSound;
    [SerializeField] private AudioSource audioSource;

    private void Start()
    {
        potionCounter.OnPotionAdded += PotionCounter_OnPotionAdded;
    }

    private void PotionCounter_OnPotionAdded(object sender, System.EventArgs e)
    {
        audioSource.PlayOneShot(potionAddSound);
    }
}
