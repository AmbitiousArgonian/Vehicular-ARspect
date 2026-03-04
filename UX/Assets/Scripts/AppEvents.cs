using System;
using UnityEngine;

public static class AppEvents
{
    // Speech / Text output
    public static event Action<string> TranscriptionReceived;

    // Images
    public static event Action<string, string> ImageCaptured;
    public static void RaiseTranscription(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;
        TranscriptionReceived?.Invoke(text);
    }

    public static void RaiseImageCaptured(string path, string label)
{
    if (string.IsNullOrEmpty(path)) return;
    ImageCaptured?.Invoke(path, label ?? "");
}
}