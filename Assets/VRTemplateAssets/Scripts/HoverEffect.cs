using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Video;

public class HoverEffect : MonoBehaviour, IPointerEnterHandler
{
    public CanvasGroup hiddenContent; // CanvasGroup for the Panel
    public VideoPlayer hiddenVideoPlayer;
    public float fadeDuration = 0.5f; // Duration of fade effect

    public void OnPointerEnter(PointerEventData eventData)
    {
        StopAllCoroutines(); // Stop any ongoing fade-out coroutine
        StartCoroutine(FadeCanvasGroup(hiddenContent, hiddenContent.alpha, 1)); // Fade in the Panel
        StartCoroutine(FadeVideoPlayer(hiddenVideoPlayer, hiddenVideoPlayer.targetCameraAlpha, 1)); // Fade in the Video
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            cg.alpha = Mathf.Lerp(start, end, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cg.alpha = end;
        cg.interactable = end == 1;
        cg.blocksRaycasts = end == 1;
    }

    private IEnumerator FadeVideoPlayer(VideoPlayer vp, float start, float end)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            vp.targetCameraAlpha = Mathf.Lerp(start, end, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        vp.targetCameraAlpha = end;
    }
}
