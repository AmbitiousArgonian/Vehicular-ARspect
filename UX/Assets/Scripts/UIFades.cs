using System.Collections;
using UnityEngine;

public class UIFader : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float fadeDuration = 0.25f;

    private Coroutine currentFade;

    public void FadeIn()
    {
        StartFade(1);
    }

    public void FadeOut()
    {
        StartFade(0);
    }

    void StartFade(float target)
    {
        if (currentFade != null)
            StopCoroutine(currentFade);

        currentFade = StartCoroutine(FadeRoutine(target));
    }

    IEnumerator FadeRoutine(float target)
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

    
    }
}
