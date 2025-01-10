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
        if (trimmedTranscription.Contains("what") && trimmedTranscription.Contains("looking at"))
        {
            // Send query with a screenshot
            UnityEngine.Debug.Log("Message: " + fullTranscription.Trim().ToLower());
            UnityEngine.Debug.Log("Sending query with screenshot");
            geminiAPIManager.SendQueryWithScreenshot(fullTranscription);
        }
        else
        {
            // Send query without screenshot
            UnityEngine.Debug.Log("Message: " + fullTranscription.Trim().ToLower());
            UnityEngine.Debug.Log("Sending query without screenshot");
            geminiAPIManager.SendQueryToGemini(fullTranscription);
        }
    }

    public void OnTTSStart()
    {
        _ttsInProgress = true;
        appVoiceExperience.Deactivate(); // Deactivate voice recognition
    }

    public void OnTTSEnd()
    {
        _ttsInProgress = false;
        Invoke(nameof(ReactivateVoice), 2f); // Reactivate voice recognition after 2 seconds
    }

    public void OnGPTResponseReceived()
    {
        // Reactivate Wit after GPT response
        ReactivateVoice();
    }
}
