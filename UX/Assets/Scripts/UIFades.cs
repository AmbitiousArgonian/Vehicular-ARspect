using System.Collections;
using UnityEngine;

public class UIFader : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float fadeDuration = 0.25f;

    private Coroutine currentFade;

    public void FadeIn()
    {
        gameObject.SetActive(true);
        StartFade(1, false);
    }

    public void FadeOut()
    {
        StartFade(0, true);
    }

    void StartFade(float target, bool disableAfter)
    {
        if (currentFade != null)
            StopCoroutine(currentFade);

        currentFade = StartCoroutine(FadeRoutine(target, disableAfter));
    }

    IEnumerator FadeRoutine(float target, bool disableAfter)
    {
        float start = canvasGroup.alpha;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, target, time / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = target;
        canvasGroup.interactable = target > 0.9f;
        canvasGroup.blocksRaycasts = target > 0.9f;

        if (disableAfter && target == 0)
            gameObject.SetActive(false);
    }
}
