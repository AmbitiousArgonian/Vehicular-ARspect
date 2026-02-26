//Fügt Text/Bilder auf einer Seite ein
using System.Collections.Generic;
using UnityEngine;

public class PDFPage
{
    private PDFDocument doc;

    public List<string> TextCommands = new List<string>();
    public List<PDFImage> Images = new List<PDFImage>();

    public PDFPage(PDFDocument document)
    {
        doc = document;
    }

   public void AddText(string text, float x, float y, int fontSize = 12)
{
    string escaped = text
        .Replace("\\", "\\\\")
        .Replace("(", "\\(")
        .Replace(")", "\\)")
        .Replace("ä", "ae")
        .Replace("ö", "oe")
        .Replace("ü", "ue")
        .Replace("Ä", "Ae")
        .Replace("Ö", "Oe")
        .Replace("Ü", "Ue")
        .Replace("ß", "ss");

    TextCommands.Add($"BT /F1 {fontSize} Tf {x} {y} Td ({escaped}) Tj ET");
}



    public void AddImage(Texture2D tex, float x, float y, float w, float h)
    {
        Images.Add(new PDFImage(tex, x, y, w, h));
    }

    private string Escape(string s)
{
    return s.Replace("(", "\\(").Replace(")", "\\)");
}


    public void AddWrappedText(string text, float x, float y, float maxWidth, int fontSize = 12)
{
    string[] paragraphs = text.Split('\n');

   
    float lineHeight = fontSize + 8;


 

foreach (string paragraph in paragraphs)
{
    string[] words = paragraph.Split(' ');
    string currentLine = "";

    foreach (string word in words)
    {
        string testLine = currentLine + word + " ";
        float testWidth = testLine.Length * (fontSize * 0.5f);

        if (testWidth > maxWidth)
        {
            AddText(currentLine, x, y, fontSize);
            y -= lineHeight;
            currentLine = word + " ";
        }
        else
        {
            currentLine = testLine;
        }
    }

    if (currentLine.Length > 0)
    {
        AddText(currentLine, x, y, fontSize);
        y -= lineHeight;
    }

    // Absatzabstand
    y -= lineHeight * 0.5f;
}

}
}
