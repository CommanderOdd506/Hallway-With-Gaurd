using UnityEngine;
using System.Collections;

public class FadingScript : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 4.0f;
    [SerializeField] private bool fadeFromBlack = true;

    private void Start()
    {
        if (fadeFromBlack)
        {
            canvasGroup.alpha = 1f; // start black
            FadeIn(); // fade to clear
        }
        else
        {
            canvasGroup.alpha = 0f; // start clear
            FadeOut(); // fade to black
        }
    }

    public void FadeIn()
    {
        StartCoroutine(FadeCanvasGroup(canvasGroup, 1f, 0f, fadeDuration));
    }

    public void FadeOut()
    {
        StartCoroutine(FadeCanvasGroup(canvasGroup, 0f, 1f, 0.3f));
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, elapsedTime / duration);
            yield return null;
        }

        cg.alpha = end;
    }
}