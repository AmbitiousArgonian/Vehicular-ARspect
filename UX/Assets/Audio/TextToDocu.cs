using System.IO;
using UnityEngine;

public class TextToDocu : MonoBehaviour
{
    [TextArea(5, 10)]
    public string textToSave;

    public void SaveText(string newText)
    {
        textToSave = newText;

        string path = Application.persistentDataPath + "/TextToDocu.txt";
        File.WriteAllText(path, textToSave);

        Debug.Log("Text gespeichert unter: " + path);
    }
}