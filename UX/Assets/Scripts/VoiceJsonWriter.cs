//Speichert voice Text in JSON

using UnityEngine;
using System.IO;
using Newtonsoft.Json.Linq;

public class VoiceJsonWriter : MonoBehaviour
{
    public string jsonPath = Application.persistentDataPath + "/report.json";

    public void SaveToJson(string text)
    {
        JObject json;

        if (File.Exists(jsonPath))
            json = JObject.Parse(File.ReadAllText(jsonPath));
        else
            json = new JObject();

        json["voice_input"] = text;

        File.WriteAllText(jsonPath, json.ToString());
        Debug.Log("JSON aktualisiert: " + jsonPath);
    }
}

