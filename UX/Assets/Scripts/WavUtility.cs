using UnityEngine;
using System.IO;
using System;

public static class WavUtility
{
    public static byte[] FromAudioClip(AudioClip clip)
    {
        var samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        // 16-bit PCM
        byte[] bytes = ConvertToWav(samples, clip.channels, clip.frequency);
        return bytes;
    }

    private static byte[] ConvertToWav(float[] samples, int channels, int sampleRate)
    {
        MemoryStream stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);

        int sampleCount = samples.Length;
        int byteRate = sampleRate * channels * 2;

        // RIFF header
        writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
        writer.Write(36 + sampleCount * 2);
        writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));

        // fmt chunk
        writer.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
        writer.Write(16);
        writer.Write((short)1); // PCM
        writer.Write((short)channels);
        writer.Write(sampleRate);
        writer.Write(byteRate);
        writer.Write((short)(channels * 2));
        writer.Write((short)16);

        // data chunk
        writer.Write(System.Text.Encoding.ASCII.GetBytes("data"));
        writer.Write(sampleCount * 2);

        // samples
        foreach (var s in samples)
        {
            short val = (short)Mathf.Clamp(s * short.MaxValue, short.MinValue, short.MaxValue);
            writer.Write(val);
        }

        writer.Flush();
        return stream.ToArray();
    }
}
