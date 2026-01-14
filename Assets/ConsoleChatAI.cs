using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ChatGPTConsoleInteractive : MonoBehaviour
{
    [SerializeField] private string apiKey;

    private string userInput = "";
    private bool isSending = false;

    // gpt-4.1-mini blended cost ≈ $0.42 / 1M tokens
    private const float COST_PER_TOKEN = 0.00000042f;

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 400, 20), "ChatGPT Console (Press Enter)");

        userInput = GUI.TextField(
            new Rect(10, 35, 400, 25),
            userInput,
            500
        );

        if (Event.current.isKey &&
            Event.current.keyCode == KeyCode.Return &&
            !isSending &&
            !string.IsNullOrEmpty(userInput))
        {
            Send(userInput);
            userInput = "";
        }
    }

    void Send(string prompt)
    {
        Debug.Log("YOU: " + prompt);
        StartCoroutine(Request(prompt));
    }

    IEnumerator Request(string prompt)
    {
        isSending = true;

        string json = $@"
        {{
            ""model"": ""gpt-4.1-nano"",
            ""input"": ""{prompt}"",
            ""max_output_tokens"": 150
        }}";

        var request = new UnityWebRequest(
            "https://api.openai.com/v1/responses",
            "POST"
        );

        request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
            isSending = false;
            yield break;
        }

        string raw = request.downloadHandler.text;
        Debug.Log("CHATGPT RAW:\n" + raw);

        int estimatedTokens = (prompt.Length / 4) + 150;
        float estimatedCost = estimatedTokens * COST_PER_TOKEN;

        Debug.Log($"Estimated tokens: {estimatedTokens}");
        Debug.Log($"Estimated cost: ${estimatedCost:F6}");

        isSending = false;
    }
}
