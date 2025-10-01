using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Onomatopoeia : MonoBehaviour
{
    private const string POPUP_TRIGGER = "NumberPopup";

    [SerializeField] private Image boomImage; // Replace TextMeshProUGUI with Image
    [SerializeField] private Sprite boomSprite; // Optional: assign a sprite for "BOOM"

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        Hide();
    }

    /// <summary>
    /// Call this method to display the image, play the animation, and then hide.
    /// </summary>
    public void Boom()
    {
        // Activate first
        Show();

        if (boomImage != null && boomSprite != null)
        {
            boomImage.sprite = boomSprite; // Set the image to the desired sprite
            boomImage.enabled = true; // Make sure the image is visible
        }

        if (animator != null)
        {
            animator.SetTrigger(POPUP_TRIGGER);
        }

        // Start coroutine after object is active
        StartCoroutine(HideAfterAnimation());
    }

    private IEnumerator HideAfterAnimation()
    {
        if (animator != null)
        {
            AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
            float duration = clipInfo.Length > 0 ? clipInfo[0].clip.length : 1f;
            yield return new WaitForSeconds(duration);
        }
        else
        {
            yield return new WaitForSeconds(1f); // default duration
        }
        Hide();
    }

    private void Show()
    {
        gameObject.SetActive(true);
        if (boomImage != null)
        {
            boomImage.enabled = true;
        }
    }

    private void Hide()
    {
        if (boomImage != null)
        {
            boomImage.enabled = false;
        }
        gameObject.SetActive(false);
    }
}
