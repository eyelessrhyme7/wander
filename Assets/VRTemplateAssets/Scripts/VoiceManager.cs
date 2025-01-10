using UnityEngine;
using UnityEngine.Events;
using TMPro;
using Oculus.Voice;
using System.Reflection;
using Meta.WitAi.CallbackHandlers;

public class VoiceManager : MonoBehaviour
{
    [Header("Wit Configuration")]
    [SerializeField] private AppVoiceExperience appVoiceExperience;
    [SerializeField] private WitResponseMatcher responseMatcher;
    [SerializeField] private TextMeshProUGUI transcriptionText;

    [Header("Voice Events")]
    [SerializeField] private UnityEvent wakeWordDetected;
    [SerializeField] private UnityEvent<string> completeTranscription;
    [SerializeField] private float fadeOutDuration = 2f; // Single duration control

    [Header("OpenAI Integration")]
    [SerializeField] private GeminiAPIManager geminiAPIManager; // Reference to OpenAIManager

    private bool _voiceCommandReady;
    private bool _ttsInProgress; // Track if TTS is in progress

    private void Awake()
    {
        appVoiceExperience.VoiceEvents.OnRequestCompleted.AddListener(ReactivateVoice);
        appVoiceExperience.VoiceEvents.OnPartialTranscription.AddListener(OnPartialTranscription);
        appVoiceExperience.VoiceEvents.OnFullTranscription.AddListener(OnFullTranscription);

        var eventField = typeof(WitResponseMatcher).GetField("onMultiValueEvent", BindingFlags.NonPublic | BindingFlags.Instance);
        if (eventField != null && eventField.GetValue(responseMatcher) is MultiValueEvent onMultiValueEvent)
        {
            onMultiValueEvent.AddListener(WakeWordDetected);
        }

        appVoiceExperience.Activate();
    }

    private void OnDestroy()
    {
        appVoiceExperience.VoiceEvents.OnRequestCompleted.RemoveListener(ReactivateVoice);
        appVoiceExperience.VoiceEvents.OnPartialTranscription.RemoveListener(OnPartialTranscription);
        appVoiceExperience.VoiceEvents.OnFullTranscription.RemoveListener(OnFullTranscription);

        var eventField = typeof(WitResponseMatcher).GetField("onMultiValueEvent", BindingFlags.NonPublic | BindingFlags.Instance);
        if (eventField != null && eventField.GetValue(responseMatcher) is MultiValueEvent onMultiValueEvent)
        {
            onMultiValueEvent.RemoveListener(WakeWordDetected);
        }
    }

    private void ReactivateVoice()
    {
        if (!_ttsInProgress) // Only activate if TTS isn't in progress
        {
            appVoiceExperience.Activate();
        }
    }

    private void WakeWordDetected(string[] arg0)
    {
        if (!_ttsInProgress) // Ignore wake word if TTS is in progress
        {
            _voiceCommandReady = true;
            wakeWordDetected.Invoke();
        }
    }

    private void OnPartialTranscription(string partialTranscription)
    {
        if (!_voiceCommandReady || _ttsInProgress) return; // Ignore if TTS is in progress
        transcriptionText.text = partialTranscription;
    }

    private void OnFullTranscription(string fullTranscription)
    {
        if (!_voiceCommandReady || _ttsInProgress) return; // Ignore if TTS is in progress
        _voiceCommandReady = false;
        transcriptionText.text = fullTranscription;

        // Process the full transcription
        appVoiceExperience.Deactivate();

        string trimmedTranscription = fullTranscription.Trim().ToLower();

        // Check if the user said "What's that?"
        if (trimmedTranscription.Contains("what") && trimmedTranscription.Contains("looking at") || trimmedTranscription.Contains("is that"))
        {
            UnityEngine.Debug.Log("Message: " + fullTranscription.Trim().ToLower());
            UnityEngine.Debug.Log("Sending query with screenshot");
            geminiAPIManager.SendQueryWithScreenshot(fullTranscription);
        }
        else
        {
            UnityEngine.Debug.Log("Message: " + fullTranscription.Trim().ToLower());
            UnityEngine.Debug.Log("Sending query without screenshot");
            geminiAPIManager.SendQueryToGemini(fullTranscription);
        }

        // Clear the transcription text after delay
        StartCoroutine(ClearTranscriptionAfterDelay());
    }

    private System.Collections.IEnumerator ClearTranscriptionAfterDelay()
    {
    // Start fading immediately
    float elapsedTime = 0;
    Color startColor = transcriptionText.color;
    Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

    while (elapsedTime < fadeOutDuration)
    {
        elapsedTime += Time.deltaTime;
        float normalizedTime = elapsedTime / fadeOutDuration;
        transcriptionText.color = Color.Lerp(startColor, targetColor, normalizedTime);
        yield return null;
    }

    // Reset the alpha and clear the text
    transcriptionText.color = startColor;
    transcriptionText.text = "";
    }

    public void OnTTSStart()
    {
        _ttsInProgress = true;
        MuteMicrophone(); // Mute the microphone
        appVoiceExperience.Deactivate(); // Deactivate voice recognition
    }

    public void OnTTSEnd()
    {
        _ttsInProgress = false;
        UnmuteMicrophone(); // Unmute the microphone
        ReactivateVoice(); // Reactivate voice recognition
    }

    private void MuteMicrophone()
    {
        AudioListener.pause = true; // Mute microphone
        UnityEngine.Debug.Log("Microphone muted.");
    }

    private void UnmuteMicrophone()
    {
        AudioListener.pause = false; // Unmute microphone
        UnityEngine.Debug.Log("Microphone unmuted.");
    }

    public void OnGPTResponseReceived()
    {
        // Reactivate Wit after GPT response
        ReactivateVoice();
    }
}
