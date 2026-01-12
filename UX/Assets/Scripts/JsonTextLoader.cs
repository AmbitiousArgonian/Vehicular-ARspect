using UnityEngine;
using TMPro;
using System.IO;

public class JsonTextLoader : MonoBehaviour
{
    public TextMeshProUGUI textField;
    public string fileName = "VehicleInfo.json";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadText();
    }

    void LoadText()
    {
        string path = Path.Combine(Application.streamingAssetsPath, fileName);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            TextData data = JsonUtility.FromJson<TextData>(json);

            textField.text = $"<b>{data.title}</b>\n\n{data.message}";
        }
        else
        {
            textField.text = "File not found.";
        }
    }
}