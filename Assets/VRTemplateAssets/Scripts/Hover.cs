using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class HoverEffectRaycast : MonoBehaviour
{
    public CanvasGroup hiddenContent; // CanvasGroup for the Panel
    public VideoPlayer hiddenVideoPlayer;
    public float fadeDuration = 0.5f; // Duration of fade effect
    public LayerMask interactableLayer; // Layer mask for interactable objects (e.g., Image, Text)

    private bool isHovering = false;

    private void Update()
    {
        DetectRayHover();
    }

    private void DetectRayHover()
    {
        // Create a ray from the controller (or camera)
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // Perform the raycast
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, interactableLayer))
        {
            if (!isHovering)
            {
                isHovering = true;
                OnHoverEnter(hit.collider.gameObject);
            }
        }
        else if (isHovering)
        {
            isHovering = false;
            OnHoverExit();
        }
    }

    private void OnHoverEnter(GameObject hitObject)
    {
        UnityEngine.Debug.Log($"Hover entered on {hitObject.name}");
        StopAllCoroutines();
        StartCoroutine(FadeCanvasGroup(hiddenContent, hiddenContent.alpha, 1));
        StartCoroutine(FadeVideoPlayer(hiddenVideoPlayer, hiddenVideoPlayer.targetCameraAlpha, 1));
    }

    private void OnHoverExit()
    {
        UnityEngine.Debug.Log("Hover exited");
        StopAllCoroutines();
        StartCoroutine(FadeCanvasGroup(hiddenContent, hiddenContent.alpha, 0));
        StartCoroutine(FadeVideoPlayer(hiddenVideoPlayer, hiddenVideoPlayer.targetCameraAlpha, 0));
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end)
    {
        if (cg == null) yield break;
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
        if (vp == null) yield break;
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
