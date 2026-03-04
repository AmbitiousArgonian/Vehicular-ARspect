using System.Collections;
using UnityEngine;
using System.IO;

public class ImageCapture : MonoBehaviour
{
    private void OnEnable()
    {
        AppEvents.TranscriptionReceived += OnTranscription;
    }

    private void OnDisable()
    {
        AppEvents.TranscriptionReceived -= OnTranscription;
    }

    private void OnTranscription(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;

        string lower = text.ToLower();

        if (lower.Contains("foto") || lower.Contains("bild") || lower.Contains("picture") || lower.Contains("photo"))
        {
            StartCoroutine(CaptureRoutine(text));
        }
    }

   private IEnumerator CaptureRoutine(string label)
{
    yield return new WaitForEndOfFrame();

    int width = Screen.width;
    int height = Screen.height;

    Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
    tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
    tex.Apply();

    byte[] png = tex.EncodeToPNG();

    string fileName = "capture_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
    string path = Path.Combine(Application.persistentDataPath, fileName);

    File.WriteAllBytes(path, png);

    Debug.Log("Image saved: " + path);

    AppEvents.RaiseImageCaptured(path, label);
}
}