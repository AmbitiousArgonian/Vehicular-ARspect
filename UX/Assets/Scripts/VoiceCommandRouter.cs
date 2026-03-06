
//VoiceCommandRouter → Entscheidet: Befehl oder normaler Text
//Whisper → TextToDocu verbinden

using UnityEngine;

public class VoiceCommandRouter : MonoBehaviour
{
    private WhisperClient whisper;
    private VoiceCommandExecutor executor;
    private TextToDocu textToDoc;

    private void Start()
    {
        whisper = FindObjectOfType<WhisperClient>();
        executor = FindObjectOfType<VoiceCommandExecutor>();
        textToDoc = FindObjectOfType<TextToDocu>();

        whisper.OnTranscriptionReceived += HandleText;
    }

    private void HandleText(string text)
    {
        string lower = text.ToLower();

        // Sprachbefehle
        if (lower.Contains("start") ||
            lower.Contains("weiter") ||
            lower.Contains("zurück") ||
            lower.Contains("zurueck") ||
            lower.Contains("abbrechen") ||
            lower.Contains("speichern"))
        {
            executor.Execute(lower);
            return;
        }

        // Freies Sprechen → direkt an docText anhängen
        textToDoc.docText += " " + text;
        Debug.Log("Text angehängt: " + text);
    }
}


