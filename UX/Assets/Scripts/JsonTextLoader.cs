using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using static System.Net.Mime.MediaTypeNames;

public class JsonTextLoader : MonoBehaviour
{
    public TextMeshProUGUI textField;
    public string fileName = "VehicleInfo.json";

    void Start()
    {
        StartCoroutine(LoadText());
    }

    IEnumerator LoadText()
    {
        string path = System.IO.Path.Combine(UnityEngine.Application.streamingAssetsPath, fileName);

#if UNITY_ANDROID && !UNITY_EDITOR
        UnityWebRequest request = UnityWebRequest.Get(path);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ProcessJson(request.downloadHandler.text);
        }
        else
        {
            textField.text = "File not found (Android).";
        }
#else
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            ProcessJson(json);
        }
        else
        {
            textField.text = "File not found.";
        }
#endif
        yield break;
    }

    void ProcessJson(string json)
    {
        TextData data = JsonUtility.FromJson<TextData>(json);
        textField.text = $"<b>{data.title}</b>\n\n{data.message}";
    }
}
