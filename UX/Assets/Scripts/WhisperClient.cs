/*using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class WhisperClient : MonoBehaviour
{
    [Header("Server Settings")]
    public string serverUrl = "http://localhost:8080/inference";

   public IEnumerator Transcribe(byte[] wavData)
{
    WWWForm form = new WWWForm();
    form.AddBinaryData("file", wavData, "audio.wav", "audio/wav");

    using (UnityWebRequest request = UnityWebRequest.Post(serverUrl, form))
    {
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Whisper result: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Whisper error: " + request.error);
        }
    }
}

}*/

using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class WhisperClient : MonoBehaviour
{
    public string serverUrl = "http://localhost:8080/inference";

    public Action<string> OnTranscriptionReceived;   // <-- NEU

    public IEnumerator Transcribe(byte[] wavData)
    {
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", wavData, "audio.wav", "audio/wav");

        using (UnityWebRequest request = UnityWebRequest.Post(serverUrl, form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string text = request.downloadHandler.text;
                Debug.Log("Whisper result: " + text);

                OnTranscriptionReceived?.Invoke(text);   // <-- NEU
            }
            else
            {
                Debug.LogError("Whisper error: " + request.error);
            }
        }
    }
}

