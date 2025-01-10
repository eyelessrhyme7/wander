using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.XR.Interaction.Toolkit;

public class HoverEffect : MonoBehaviour
{
    public CanvasGroup hiddenContent; // CanvasGroup for the Panel (optional)
    public VideoPlayer hiddenVideoPlayer;
    public float fadeDuration = 0.5f; // Duration of fade effect

    private void OnEnable()
    {
        // Subscribe to hover events
        var interactable = GetComponent<XRBaseInteractable>();
        if (interactable != null)
        {
            UnityEngine.Debug.Log($"HoverEffect: Subscribed to hover events on {gameObject.name}");
            interactable.hoverEntered.AddListener(OnHoverEntered);
            interactable.hoverExited.AddListener(OnHoverExited);
        }
        else
        {
            UnityEngine.Debug.LogError($"HoverEffect: XRBaseInteractable missing on {gameObject.name}. Please add it.");
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from hover events
        var interactable = GetComponent<XRBaseInteractable>();
        if (interactable != null)
        {
            interactable.hoverEntered.RemoveListener(OnHoverEntered);
            interactable.hoverExited.RemoveListener(OnHoverExited);
        }
    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        UnityEngine.Debug.Log($"Hover entered on {gameObject.name}.");
        StopAllCoroutines();
        StartCoroutine(FadeCanvasGroup(hiddenContent, hiddenContent.alpha, 1));
        StartCoroutine(FadeVideoPlayer(hiddenVideoPlayer, hiddenVideoPlayer.targetCameraAlpha, 1));
    }

    private void OnHoverExited(HoverExitEventArgs args)
    {
        UnityEngine.Debug.Log($"Hover exited on {gameObject.name}.");
        StopAllCoroutines();
        StartCoroutine(FadeCanvasGroup(hiddenContent, hiddenContent.alpha, 0));
        StartCoroutine(FadeVideoPlayer(hiddenVideoPlayer, hiddenVideoPlayer.targetCameraAlpha, 0));
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end)
    {
        if (cg == null) yield break; // Skip if no CanvasGroup
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
        if (vp == null) yield break; // Skip if no VideoPlayer
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
