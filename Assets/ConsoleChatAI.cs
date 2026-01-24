 using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class DeepSeekChatEnter : MonoBehaviour
{
    public TMP_InputField sendInput;
    public TMP_Text receiveText;

    private const string apiUrl = "https://openrouter.ai/api/v1/chat/completions";
    private const string modelName = "deepseek/deepseek-chat";

    // 🔑 PUT YOUR OPENROUTER API KEY HERE
    private string apiKey = "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";

    void Start()
    {
        Debug.Log("[DeepSeek] Ready");
        sendInput.lineType = TMP_InputField.LineType.SingleLine;
        sendInput.onSubmit.AddListener(OnSubmit);
        sendInput.ActivateInputField();
    }

    void OnSubmit(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return;

        receiveText.text = "Thinking...";
        StartCoroutine(SendToDeepSeek(text));

        sendInput.text = "";
        sendInput.ActivateInputField();
    }

    IEnumerator SendToDeepSeek(string prompt)
{
    string json =
    @"{
        ""model"": ""mistralai/mistral-7b-instruct"",
        ""messages"": [
            { ""role"": ""user"", ""content"": """ + Escape(prompt) + @""" }
        ]
    }";

    Debug.Log("[DeepSeek] JSON: " + json);

    byte[] body = Encoding.UTF8.GetBytes(json);

    UnityWebRequest request =
        new UnityWebRequest("https://openrouter.ai/api/v1/chat/completions", "POST");

    request.uploadHandler = new UploadHandlerRaw(body);
    request.downloadHandler = new DownloadHandlerBuffer();

    // 🔑 REQUIRED HEADERS
    request.SetRequestHeader("Authorization", "Bearer " + apiKey);
    request.SetRequestHeader("Content-Type", "application/json");

    // 🚨 THESE TWO ARE REQUIRED BY OPENROUTER
    request.SetRequestHeader("HTTP-Referer", "http://localhost");
    request.SetRequestHeader("X-Title", "Unity DeepSeek Chat");

    yield return request.SendWebRequest();

    if (request.result != UnityWebRequest.Result.Success)
    {
        Debug.LogError("[DeepSeek] HTTP Error: " + request.responseCode);
        Debug.LogError("[DeepSeek] Body: " + request.downloadHandler.text);
        receiveText.text = "Error: " + request.responseCode;
        yield break;
    }

    Debug.Log("[DeepSeek] Raw response: " + request.downloadHandler.text);
    receiveText.text = ExtractContent(request.downloadHandler.text);
}

    // Simple JSON text extraction (no library needed)
    string ExtractContent(string json)
    {
        string key = "\"content\":\"";
        int start = json.IndexOf(key);
        if (start == -1) return json;

        start += key.Length;
        int end = json.IndexOf("\"", start);
        return json.Substring(start, end - start).Replace("\\n", "\n");
    }

    string Escape(string s)
    {
        return s.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
}

 
 /*using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class OllamaChatEnter : MonoBehaviour
{
    public TMP_InputField sendInput;
    public TMP_Text receiveText;

    private const string ollamaUrl = "http://localhost:11434/api/generate";
    private const string modelName = "llama3";

    void Start()
    {
        Debug.Log("[Ollama] Chat ready");
        sendInput.lineType = TMP_InputField.LineType.SingleLine;

        sendInput.onSubmit.AddListener(OnSubmit);
        sendInput.ActivateInputField();
    }

    void OnSubmit(string text)
    {
        Debug.Log("[Ollama] Submit detected");

        if (string.IsNullOrWhiteSpace(text))
        {
            Debug.LogWarning("[Ollama] Empty input");
            sendInput.ActivateInputField();
            return;
        }

        receiveText.text = "Thinking...";
        StartCoroutine(SendToOllama(text));

        sendInput.text = "";
        sendInput.ActivateInputField();
    }

    IEnumerator SendToOllama(string prompt)
    {
        Debug.Log("[Ollama] Sending: " + prompt);

        OllamaRequest requestData = new OllamaRequest
        {
            model = modelName,
            prompt = prompt,
            stream = false
        };

        string json = JsonUtility.ToJson(requestData);
        Debug.Log("[Ollama] JSON: " + json);

        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using UnityWebRequest request = new UnityWebRequest(ollamaUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("[Ollama] Error: " + request.error);
            receiveText.text = "Error: " + request.error;
        }
        else
        {
            string raw = request.downloadHandler.text;
            Debug.Log("[Ollama] Raw response: " + raw);

            OllamaResponse response =
                JsonUtility.FromJson<OllamaResponse>(raw);

            receiveText.text = response.response;
        }
    }
}

[System.Serializable]
public class OllamaRequest
{
    public string model;
    public string prompt;
    public bool stream;
}

[System.Serializable]
public class OllamaResponse
{
    public string response;
}
*/