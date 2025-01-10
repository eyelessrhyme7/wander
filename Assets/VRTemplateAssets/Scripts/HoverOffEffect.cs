using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.XR.Interaction.Toolkit;

public class HoverOffEffect : MonoBehaviour
{
    public CanvasGroup hiddenContent; // CanvasGroup for the Panel
    public VideoPlayer hiddenVideoPlayer;
    public float fadeDuration = 0.5f; // Duration of fade effect

    private void OnEnable()
    {
        // Subscribe to hover events
        var xrInteractable = GetComponent<XRBaseInteractable>();
        if (xrInteractable != null)
        {
            Debug.Log($"HoverOffEffect attached to {gameObject.name}. Subscribing to events.");
            xrInteractable.hoverExited.AddListener(OnHoverExit);
        }
        else
        {
            Debug.LogError($"No XRBaseInteractable found on {gameObject.name}. Please attach it.");
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from hover events
        var xrInteractable = GetComponent<XRBaseInteractable>();
        if (xrInteractable != null)
        {
            xrInteractable.hoverExited.RemoveListener(OnHoverExit);
        }
    }

    private void OnHoverExit(HoverExitEventArgs args)
    {
        Debug.Log($"Hover exited on {gameObject.name}");
        StartCoroutine(FadeCanvasGroup(hiddenContent, hiddenContent.alpha, 0)); // Fade out the Panel
        StartCoroutine(FadeVideoPlayer(hiddenVideoPlayer, hiddenVideoPlayer.targetCameraAlpha, 0)); // Fade out the Video
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end)
    {
        if (cg == null) yield break; // Skip if no CanvasGroup assigned
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
        if (vp == null) yield break; // Skip if no VideoPlayer assigned
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
