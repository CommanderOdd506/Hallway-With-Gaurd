using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadingScript : MonoBehaviour
{
    [SerializeField] private RawImage fadeImage;
    [SerializeField] private float fadeDuration = 4.0f;
    [SerializeField] private bool fadeFromBlack = true;

    private void Start()
    {
        if (fadeFromBlack)
        {
            SetAlpha(1f);
            FadeIn();
        }
        else
        {
            SetAlpha(0f);
            FadeOut();
        }
    }

    /// <summary>Fade the image to transparent.</summary>
    public void FadeIn()
    {
        StartCoroutine(FadeRoutine(1f, 0f, fadeDuration));
    }

    /// <summary>Fade the image to opaque.</summary>
    public void FadeOut()
    {
        StartCoroutine(FadeRoutine(0f, 1f, 0.3f));
    }

    private void SetAlpha(float alpha)
    {
        Color c = fadeImage.color;
        c.a = alpha;
        fadeImage.color = c;
    }

    private IEnumerator FadeRoutine(float start, float end, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            SetAlpha(Mathf.Lerp(start, end, elapsed / duration));
            yield return null;
        }

        SetAlpha(end);
    }
}
