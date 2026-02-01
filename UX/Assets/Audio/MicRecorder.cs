using UnityEngine;
using UnityEngine.InputSystem;   // <- NEU
using System.Collections;
using System.Linq;

public class MicRecorder : MonoBehaviour
{
    [Header("Recording Settings")]
    public int sampleRate = 44100;
    public int recordDuration = 5;

    private AudioClip _clip;
    private string device;
    private WhisperClient _client;

    private bool isRecording = false;

    private void Start()
    {
        // --- WICHTIG: macOS AudioSession initialisieren ---
        var config = AudioSettings.GetConfiguration();
        config.sampleRate = 44100;
        AudioSettings.Reset(config);

        Debug.Log("AudioSession initialisiert. SampleRate: " + config.sampleRate);

        // --- Devices loggen ---
        for (int i = 0; i < Microphone.devices.Length; i++)
            Debug.Log($"[{i}] {Microphone.devices[i]}");

        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("Kein Mikrofon gefunden!");
            return;
        }

        // --- MacBook Mikrofon per Index erzwingen ---
        int deviceIndex = -1;
        for (int i = 0; i < Microphone.devices.Length; i++)
        {
            if (Microphone.devices[i].Contains("MacBook"))
                deviceIndex = i;
        }

        if (deviceIndex == -1)
        {
            Debug.LogError("MacBook-Mikrofon nicht gefunden!");
            return;
        }

        device = Microphone.devices[deviceIndex];
        Debug.Log("Benutze Mikrofon: " + device);

        _client = FindObjectOfType<WhisperClient>();
        if (_client == null)
            Debug.LogError("Kein WhisperClient gefunden!");
    }

    private void Update()
    {
        // Neu: Input System statt Input.GetKeyDown
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            StartRecording();
    }

    public void StartRecording()
    {
        if (isRecording)
        {
            Debug.Log("Aufnahme läuft bereits.");
            return;
        }

        isRecording = true;

        Debug.Log("Starte Aufnahme...");

        // --- WICHTIG: 44100 Hz verwenden ---
        _clip = Microphone.Start(device, false, recordDuration + 1, sampleRate);

        StartCoroutine(WaitForMicThenRecord());
    }

    private IEnumerator WaitForMicThenRecord()
    {
        // Warten bis das Mikrofon wirklich läuft
        while (Microphone.GetPosition(device) <= 0)
            yield return null;

        Debug.Log("Mikrofon liefert jetzt echte Samples.");

        StartCoroutine(StopAndSend());
    }

    private IEnumerator StopAndSend()
    {
        yield return new WaitForSeconds(recordDuration);

        int position = Microphone.GetPosition(device);

        if (position <= 0)
        {
            Debug.LogWarning("Keine Samples aufgenommen – Abbruch");
            isRecording = false;
            yield break;
        }

        Microphone.End(device);

        // --- WICHTIG: macOS braucht Zeit, um den Clip zu finalisieren ---
        yield return new WaitForSeconds(0.3f);

        Debug.Log("Clip Info: channels=" + _clip.channels + " freq=" + _clip.frequency);

        if (_clip.channels == 0 || _clip.frequency == 0)
        {
            Debug.LogError("AudioClip hat ungültiges Format – macOS liefert keine Daten.");
            isRecording = false;
            yield break;
        }

        position = Mathf.Min(position, _clip.samples);

        Debug.Log("Recorded samples: " + position);

        float[] samples = new float[position * _clip.channels];
        _clip.GetData(samples, 0);

        Debug.Log("First sample: " + samples[0]);

        float max = samples.Max(s => Mathf.Abs(s));
        Debug.Log("Max amplitude: " + max);

        if (max < 0.01f)
        {
            Debug.LogWarning("Audio ist praktisch stumm – wird nicht gesendet");
            isRecording = false;
            yield break;
        }

        AudioClip trimmedClip = AudioClip.Create(
            "trimmed",
            position,
            _clip.channels,
            sampleRate,
            false
        );

        trimmedClip.SetData(samples, 0);

        byte[] wavData = WavUtility.FromAudioClip(trimmedClip);
        Debug.Log("WAV bytes: " + wavData.Length);

        StartCoroutine(_client.Transcribe(wavData));

        isRecording = false;
    }
}
