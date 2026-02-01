using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TextToDocu : MonoBehaviour
{
    [TextArea(5, 10)]
    public string textToSave;

    [Header("References")]
    // Nimmt Screenshots auf und liefert Texture2D zurück
    public ImageCapture imageCapture;

    // Erzeugt die PDF anhand des transkribierten Textes und der Bilder
    public TUVTemplatePro tuvTemplate;

    [Header("Output")]
    // Speichert den zuletzt empfangenen Text lokal (Debug/Protokollierung)
    public string outputFileName = "TextToDocu.txt";

    // Gesammelte Bilder, die in die PDF übernommen werden sollen
    private readonly List<Texture2D> capturedImages = new List<Texture2D>();

    private string outputPath;

    private void Awake()
    {
        outputPath = Path.Combine(Application.persistentDataPath, outputFileName);

        // Optional: Listener registrieren, falls Bilder asynchron über Event geliefert werden
        // (CaptureWithCallback kann alternativ direkt genutzt werden.)
        if (imageCapture != null)
        {
            imageCapture.OnImageCaptured += HandleImageCaptured;
        }
    }

    private void OnDestroy()
    {
        // Listener entfernen, um doppelte Registrierungen bei Szenenwechsel zu vermeiden
        if (imageCapture != null)
        {
            imageCapture.OnImageCaptured -= HandleImageCaptured;
        }
    }

    /// <summary>
    /// Empfangspunkt für transkribierten Text (z. B. aus WhisperClient).
    /// Speichert Text lokal und aktualisiert die PDF.
    /// Falls der Text einen Foto-Befehl enthält, wird zusätzlich eine Bildaufnahme gestartet.
    /// </summary>
    public void SaveText(string newText)
    {
        if (string.IsNullOrWhiteSpace(newText))
            return;

        textToSave = newText;

        // Text wird zur Nachvollziehbarkeit in eine Datei geschrieben
        File.WriteAllText(outputPath, newText);

        string lower = newText.ToLowerInvariant();

        bool isCaptureCommand =
            lower.Contains("bild aufnehmen") ||
            lower.Contains("foto machen") ||
            lower.Contains("bild machen") ||
            lower.Contains("take a picture") ||
            lower.Contains("take picture") ||
            lower.Contains("make a photo") ||
            lower.Contains("make photo");

        // Wenn es ein Foto-Befehl ist: Screenshot starten.
        // Die PDF wird nach der Aufnahme aktualisiert (siehe HandleImageCaptured / Callback).
        if (isCaptureCommand)
        {
            if (imageCapture == null)
            {
                Debug.LogWarning("TextToDocu: ImageCapture ist nicht zugewiesen.");
                return;
            }

            // Variante über Callback 
            imageCapture.CaptureWithCallback(newText, tex =>
            {
                if (tex != null)
                {
                    capturedImages.Add(tex);
                }

                UpdatePdf(newText);
            });

            return;
        }

        // Normaler Text: PDF direkt aktualisieren (ohne neue Bildaufnahme)
        UpdatePdf(newText);
    }

    /// <summary>
    /// Event-Handler, falls ImageCapture Bilder über das Event liefert.
    /// Diese Variante ist sinnvoll, wenn Capture nicht ausschließlich über SaveText getriggert wird.
    /// </summary>
   /** private void HandleImageCaptured(Texture2D tex)
    {
        if (tex == null)
            return;

        capturedImages.Add(tex);

        // Falls das Bild unabhängig vom Text ausgelöst wurde,
        // wird die PDF mit dem zuletzt bekannten Text aktualisiert.
        UpdatePdf(textToSave);
    }*/
    private void HandleImageCaptured(Texture2D tex)
{
    Debug.Log("TextToDocu: HandleImageCaptured CALLED. tex=" + (tex ? "OK" : "NULL"));

    if (tex != null)
        Debug.Log($"TextToDocu: Captured texture size = {tex.width}x{tex.height}, format={tex.format}");

    if (tex == null)
        return;

    capturedImages.Add(tex);
    Debug.Log("TextToDocu: capturedImages.Count = " + capturedImages.Count);

    UpdatePdf(textToSave);
}


    /// <summary>
    /// Übergibt Text und gesammelte Bilder an das PDF-Template und erzeugt die PDF neu.
    /// </summary>
    private void UpdatePdf(string currentText)
    {
        if (tuvTemplate == null)
        {
            Debug.LogWarning("TextToDocu: TUVTemplatePro ist nicht zugewiesen.");
            return;
        }

        tuvTemplate.SetInput(currentText, capturedImages);
        tuvTemplate.CreateTUVTemplate();
    }
}



/**
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TextToDocu : MonoBehaviour
{
    [TextArea(5, 10)]
    public string textToSave;

    [Header("References")]
    public ImageCapture imageCapture;
    public TUVTemplatePro tuvTemplate;

    [Header("Output")]
    public string outputFileName = "TextToDocu.txt";

    private readonly List<Texture2D> capturedImages = new List<Texture2D>();
    private string outputPath;

    private void Awake()
    {
        outputPath = Path.Combine(Application.persistentDataPath, outputFileName);
    }

    public void SaveText(string newText)
    {
        if (string.IsNullOrWhiteSpace(newText))
            return;

        textToSave = newText;
        File.WriteAllText(outputPath, newText);

        string lower = newText.ToLowerInvariant();

        bool isCaptureCommand =
            lower.Contains("bild aufnehmen") ||
            lower.Contains("foto machen") ||
            lower.Contains("bild machen") ||
            lower.Contains("take a picture") ||
            lower.Contains("take picture") ||
            lower.Contains("make a photo") ||
            lower.Contains("make photo");

        if (isCaptureCommand)
        {
            if (imageCapture == null)
            {
                Debug.LogWarning("TextToDocu: ImageCapture ist nicht zugewiesen.");
                return;
            }

            imageCapture.CaptureWithCallback(newText, tex =>
            {
                if (tex != null)
                {
                    capturedImages.Add(tex);
                    Debug.Log("TextToDocu: Bild hinzugefügt. Count=" + capturedImages.Count);
                }
                else
                {
                    Debug.LogWarning("TextToDocu: Capture lieferte null Texture.");
                }

                UpdatePdf(newText);
            });

            return;
        }

        UpdatePdf(newText);
    }

    private void UpdatePdf(string currentText)
    {
        if (tuvTemplate == null)
        {
            Debug.LogWarning("TextToDocu: TUVTemplatePro ist nicht zugewiesen.");
            return;
        }

        tuvTemplate.SetInput(currentText, capturedImages);
        tuvTemplate.CreateTUVTemplate();
        Debug.Log("TextToDocu: PDF aktualisiert.");
    }
}
*/