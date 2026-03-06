//Wandelt Texture2D → PDF‑Bild
using UnityEngine;

public class PDFImage
{
    public byte[] RawData;
    public int Width;
    public int Height;

    public float X;
    public float Y;
    public float W;
    public float H;

    public PDFImage(Texture2D tex, float x, float y, float w, float h)
    {
        RawData = tex.EncodeToJPG();
        Width = tex.width;
        Height = tex.height;

        X = x;
        Y = y;
        W = w;
        H = h;
    }
}
