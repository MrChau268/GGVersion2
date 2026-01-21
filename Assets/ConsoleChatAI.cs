using System.Collections;
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
