using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

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
        Debug.Log("PATH: " + path);

#if UNITY_ANDROID && !UNITY_EDITOR
        UnityWebRequest request = UnityWebRequest.Get(path);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("JSON LOADED");
            ProcessJson(request.downloadHandler.text);
        }
        else
        {
            Debug.Log("ERROR: " + request.error);
            textField.text = "File not found (Android).";
        }
#else
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            Debug.Log("JSON LOADED");
            ProcessJson(json);
        }
        else
        {
            Debug.Log("FILE NOT FOUND");
            textField.text = "File not found.";
        }
#endif

        yield break;
    }

    void ProcessJson(string json)
    {
        TextData data = JsonUtility.FromJson<TextData>(json);

        if (data == null)
        {
            textField.text = "JSON Parse Error";
            return;
        }

        textField.text = $"<b>{data.title}</b>\n\n{data.message}";
    }
}
