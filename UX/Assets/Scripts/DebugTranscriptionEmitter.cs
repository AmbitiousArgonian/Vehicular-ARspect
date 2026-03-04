using UnityEngine;
using UnityEngine.InputSystem;

public class DebugTranscriptionEmitter : MonoBehaviour
{
    [TextArea]
    public string testText = "Foto machen";

    void Update()
    {
        // Taste T = feuert das Transcription-Event
        if (Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame)
        {
            AppEvents.RaiseTranscription(testText);
            Debug.Log("Raised transcription: " + testText);
        }
    }
}