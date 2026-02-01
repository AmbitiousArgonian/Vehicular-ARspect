using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class ImageCapture : MonoBehaviour
{
    // Kamera, aus der der Screenshot erzeugt wird (z. B. XR- oder Main Camera)
    public Camera xrCamera;

    // Optionales Debug-Flag: Screenshot zusätzlich als PNG speichern
    [Header("Debug Options")]
    public bool savePngToDisk = true;

    // Event zur Weitergabe des aufgenommenen Bildes (an PDF-Logik)
    public Action<Texture2D> OnImageCaptured;

    /// <summary>
    /// Wird von der Sprachverarbeitung aufgerufen.
    /// Prüft, ob der transkribierte Text einen Screenshot-Befehl enthält.
    /// </summary>
    public void OnVoiceCommand(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return;

        string lower = text.ToLowerInvariant();

        // Unterstützte deutsche Befehle
        bool german =
            lower.Contains("bild aufnehmen") ||
            lower.Contains("foto machen") ||
            lower.Contains("bild machen");

        // Unterstützte englische Befehle (z. B. durch automatische Übersetzung)
        bool english =
            lower.Contains("take a picture") ||
            lower.Contains("take picture") ||
            lower.Contains("make a photo") ||
            lower.Contains("make photo");

        if (german || english)
        {
            StartCoroutine(CaptureRoutine(text, null));
        }
    }

    /// <summary>
    /// Testmethode für den Editor (z. B. über Custom Inspector oder Taste).
    /// </summary>
    public void TestCapture()
    {
        OnVoiceCommand("take a picture");
    }

    /// <summary>
    /// Führt eine Bildaufnahme aus und gibt das Ergebnis über einen Callback zurück.
    /// </summary>
    public void CaptureWithCallback(string label, Action<Texture2D> onDone)
    {
        StartCoroutine(CaptureRoutine(label, onDone));
    }

    // Tastatur-Shortcut für Debug-Zwecke (Editor/PC)
    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame)
        {
            TestCapture();
        }
    }

    /// <summary>
    /// Coroutine zur eigentlichen Bildaufnahme aus der Kamera.
    /// </summary>
    private IEnumerator CaptureRoutine(string label, Action<Texture2D> onDone)
    {
        // Warten bis das aktuelle Frame vollständig gerendert ist
        yield return new WaitForEndOfFrame();

        int w = Screen.width;
        int h = Screen.height;

        if (xrCamera == null)
            xrCamera = Camera.main;

        RenderTexture rt = new RenderTexture(w, h, 24);
        xrCamera.targetTexture = rt;
        xrCamera.Render();

        RenderTexture.active = rt;

        Texture2D tex = new Texture2D(w, h, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, w, h), 0, 0);
        tex.Apply();

        xrCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        // Optional: Bild zusätzlich als PNG-Datei speichern
        if (savePngToDisk)
        {
            string safeLabel = MakeSafe(label);
            string fileName = $"capture_{safeLabel}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            string path = Path.Combine(Application.persistentDataPath, fileName);
            File.WriteAllBytes(path, tex.EncodeToPNG());
        }

        // Weitergabe des Bildes an registrierte Listener
        OnImageCaptured?.Invoke(tex);

        // Optionaler direkter Callback für den aufrufenden Code
        onDone?.Invoke(tex);
    }

    // Hilfsmethode zur Erzeugung eines dateisicheren Namens
    private string MakeSafe(string s)
    {
        if (string.IsNullOrEmpty(s))
            return "capture";

        foreach (var c in Path.GetInvalidFileNameChars())
            s = s.Replace(c, '_');

        return s.Replace(" ", "_");
    }
}
