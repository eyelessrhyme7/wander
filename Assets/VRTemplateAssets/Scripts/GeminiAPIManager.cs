using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using Newtonsoft.Json.Linq;
using Meta.WitAi.TTS.Utilities;
using System.IO;

public class GeminiAPIManager : MonoBehaviour
{
    [Header("Gemini API Configuration")]
    [SerializeField] private string geminiApiKey; // Your Gemini API Key
    [SerializeField] private TMPro.TextMeshProUGUI responseText; // UI element to display the response
    [SerializeField] private float wordDelay = 0.1f; // Delay between each word
    [SerializeField] private Camera playerCamera; // Main Camera to capture the player's view

    [Header("Text-to-Speech")]
    [SerializeField] private TTSSpeaker ttsSpeaker; // Reference to the Wit TTS Speaker component

    public void SendQueryToGemini(string userQuery)
    {
        StartCoroutine(SendQueryCoroutine(userQuery));
    }

        private IEnumerator SendQueryCoroutine(string userQuery)
    {
        string endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={geminiApiKey}";

        // Build the JSON payload with text query and generation config
        string payload = $@"{{
            ""contents"": [{{""parts"":[{{""text"": ""{userQuery}""}}]}}],
            ""generationConfig"": {{
                ""maxOutputTokens"": 50,
                ""temperature"": 0.7
            }}
        }}";
    

        using (UnityWebRequest request = new UnityWebRequest(endpoint, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(payload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Gemini API Response: " + request.downloadHandler.text);
                DisplayResponse(request.downloadHandler.text);

                // Notify VoiceManager that GPT response is received
                FindObjectOfType<VoiceManager>().OnGPTResponseReceived();
            }
            else
            {
                Debug.LogError("Error: " + request.error + "\n" + request.downloadHandler.text);
            }
        }
    }

    public void SendQueryWithScreenshot(string userQuery)
{
    StartCoroutine(SendQueryWithScreenshotCoroutine(userQuery));
}

    private IEnumerator SendQueryWithScreenshotCoroutine(string userQuery)
    {
        string endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={geminiApiKey}";

        string base64Image = CaptureCameraView();

        string payload = $@"{{
            ""contents"": [{{""parts"":[
                {{""text"": ""You are an enthusiastic tour guide giving tours in Egypt. You're speaking to an architecture enthusiast who is showing you what they see. Treat whatever you see in the image as though it is reality that you are both experiencing together. Describe what you see in the image with enthusiasm as though you are there with them, and share an interesting architectural fact related to what's visible. Keep your response concise and engaging in 2 sentences. Here is their question: {userQuery}""}},
                {{""inline_data"": {{""mime_type"": ""image/jpeg"", ""data"": ""{base64Image}""}}}}
            ]}}],
            ""generationConfig"": {{
                ""maxOutputTokens"": 50,
                ""temperature"": 0.7
            }}
        }}";
    
    using (UnityWebRequest request = new UnityWebRequest(endpoint, "POST"))
    {
        byte[] bodyRaw = Encoding.UTF8.GetBytes(payload);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            UnityEngine.Debug.Log("Gemini API Response: " + request.downloadHandler.text);
            DisplayResponse(request.downloadHandler.text);

            // Notify VoiceManager that GPT response is received
            FindObjectOfType<VoiceManager>().OnGPTResponseReceived();
        }
        else
        {
            UnityEngine.Debug.LogError("Error: " + request.error + "\n" + request.downloadHandler.text);
        }
    }
}


    private string CaptureCameraView()
    {
        // Create a RenderTexture
        RenderTexture renderTexture = new RenderTexture(1920, 1080, 24);
        playerCamera.targetTexture = renderTexture;

        // Render the camera's view
        Texture2D screenshot = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        playerCamera.Render();
        RenderTexture.active = renderTexture;

        // Read pixels from the RenderTexture
        screenshot.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        screenshot.Apply();

        // Clean up
        playerCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);

        // Encode to PNG and convert to Base64
        byte[] imageBytes = screenshot.EncodeToPNG();
        return System.Convert.ToBase64String(imageBytes);
    }

    public void DisplayResponse(string jsonResponse)
    {
        try
        {
            // Parse the JSON response
            JObject parsedJson = JObject.Parse(jsonResponse);

            // Extract the relevant text
            string relevantText = (string)parsedJson["candidates"][0]["content"]["parts"][0]["text"];

            // Start the word-by-word reveal coroutine
            StartCoroutine(ShowTextWordByWord(relevantText.Trim()));

            // Use Wit.ai Speaker for TTS
            SpeakResponse(relevantText.Trim());
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error parsing response: " + ex.Message);
            responseText.text = "Error: Unable to parse response.";
        }
    }

    private IEnumerator ShowTextWordByWord(string fullText)
    {
        responseText.text = ""; // Clear the current text
        string[] words = fullText.Split(' '); // Split the text into words

        foreach (string word in words)
        {
            responseText.text += word + " "; // Add the word to the responseText
            yield return new WaitForSeconds(wordDelay); // Wait before adding the next word
        }
    }

    private void SpeakResponse(string response)
    {
        if (ttsSpeaker != null)
        {
            ttsSpeaker.Speak(response); // Use Wit.ai TTS to speak the response
        }
        else
        {
            Debug.LogWarning("TTS Speaker not assigned!");
        }
    }
}
