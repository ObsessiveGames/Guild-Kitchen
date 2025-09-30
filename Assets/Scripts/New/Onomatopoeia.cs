using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Onomatopoeia : MonoBehaviour
{
    private const string POPUP_TRIGGER = "NumberPopup";

    [SerializeField] private TextMeshProUGUI boomText;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        Hide();
    }

    /// <summary>
    /// Call this method to display "BOOM", play the animation, and then hide.
    /// </summary>
    public void Boom()
    {
        // Activate first
        Show();

        if (boomText != null)
        {
            boomText.text = "BOOM";
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
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
