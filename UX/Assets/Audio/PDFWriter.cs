// Schreibt die PDF-Struktur (Header, XRef, Streams)
using System;
using System.Collections.Generic;
using System.Text;

public class PDFWriter
{
    private StringBuilder body = new StringBuilder();
    private List<long> xref = new List<long>();

    // Bilddaten (JPEG bytes) pro Objekt-ID
    private Dictionary<int, byte[]> imageDataByObjectId = new Dictionary<int, byte[]>();

    // ID des /Pages-Objekts (damit /Parent korrekt ist)
    private int pagesRootId = 0;

    public void StartDocument()
    {
        body.Append("%PDF-1.4\n");
    }

    /// <summary>
    /// Muss vor WritePage() aufgerufen werden, damit /Parent korrekt ist.
    /// </summary>
    public void BeginPagesRoot()
    {
        pagesRootId = NewObject();
        // Inhalt wird später in WritePages() geschrieben
    }

    public int WritePage(PDFPage page)
    {
        if (pagesRootId == 0)
            throw new Exception("PDFWriter: BeginPagesRoot() muss vor WritePage() aufgerufen werden.");

        int pageId = NewObject();

        // Content stream
        StringBuilder content = new StringBuilder();
        content.Append("BT\n");
        foreach (var cmd in page.TextCommands)
            content.Append(cmd + "\n");
        content.Append("ET\n");

        // Bilder: IDs sammeln + Draw-Commands
        List<int> imageObjectIds = new List<int>();
        foreach (var img in page.Images)
        {
            int imgId = WriteImage(img);
            imageObjectIds.Add(imgId);

            // Zeichnen über XObject-Name /Im<imgId>
            content.Append($"q\n{img.W} 0 0 {img.H} {img.X} {img.Y} cm\n/Im{imgId} Do\nQ\n");
        }

        int contentId = WriteStream(content.ToString());

        // Resources/XObject Dictionary bauen
        StringBuilder xObjectDict = new StringBuilder();
        foreach (var id in imageObjectIds)
            xObjectDict.Append($"/Im{id} {id} 0 R ");

        // Page obj (mit korrektem Parent + Resources)
        body.Append($"{pageId} 0 obj\n");
        body.Append("<< /Type /Page ");
        body.Append($"/Parent {pagesRootId} 0 R ");
        body.Append($"/Resources << /ProcSet [/PDF /Text /ImageC] /XObject << {xObjectDict} >> >> ");
        body.Append($"/Contents {contentId} 0 R ");
        body.Append(">>\n");
        body.Append("endobj\n");

        return pageId;
    }

    public int WriteImage(PDFImage img)
    {
        int id = NewObject();

        // Bilddaten merken (als echte Bytes!)
        imageDataByObjectId[id] = img.RawData;

        body.Append($"{id} 0 obj\n");
        body.Append("<< /Type /XObject /Subtype /Image ");
        body.Append($"/Width {img.Width} /Height {img.Height} ");
        body.Append("/ColorSpace /DeviceRGB /BitsPerComponent 8 ");
        body.Append($"/Filter /DCTDecode /Length {img.RawData.Length} >>\n");
        body.Append("stream\n");

        // Platzhalter, wird später durch echte Bytes ersetzt
        body.Append($"__IMG_{id}__\n");

        body.Append("endstream\nendobj\n");

        return id;
    }

    /// <summary>
    /// Schreibt den /Pages Root mit Kids und Count.
    /// Muss nach allen WritePage-Aufrufen kommen.
    /// </summary>
    public int WritePages(List<int> pageIds)
    {
        if (pagesRootId == 0)
            throw new Exception("PDFWriter: BeginPagesRoot() muss vor WritePages() aufgerufen werden.");

        // Jetzt den bereits reservierten pagesRootId füllen
        body.Append($"{pagesRootId} 0 obj\n");
        body.Append("<< /Type /Pages /Kids [");
        foreach (var pid in pageIds)
            body.Append($"{pid} 0 R ");
        body.Append($"] /Count {pageIds.Count} >>\n");
        body.Append("endobj\n");

        return pagesRootId;
    }

    public int WriteCatalog(int pagesId)
    {
        int id = NewObject();
        body.Append($"{id} 0 obj\n");
        body.Append($"<< /Type /Catalog /Pages {pagesId} 0 R >>\n");
        body.Append("endobj\n");
        return id;
    }

    public int WriteStream(string content)
    {
        int id = NewObject();

        byte[] bytes = Encoding.ASCII.GetBytes(content);

        body.Append($"{id} 0 obj\n");
        body.Append($"<< /Length {bytes.Length} >>\n");
        body.Append("stream\n");
        body.Append(content);
        body.Append("\nendstream\nendobj\n");

        return id;
    }

    public byte[] EndDocument(int catalogId)
    {
        // Body als Text (mit Platzhaltern)
        string bodyText = body.ToString();

        // Final als Bytes bauen
        List<byte> finalBytes = new List<byte>();

        void AddAscii(string s) => finalBytes.AddRange(Encoding.ASCII.GetBytes(s));

        // Placeholder-Replacement: __IMG_<id>__ -> echte JPEG bytes
        int idx = 0;
        while (idx < bodyText.Length)
        {
            int ph = bodyText.IndexOf("__IMG_", idx, StringComparison.Ordinal);
            if (ph == -1)
            {
                AddAscii(bodyText.Substring(idx));
                break;
            }

            // Text vor Placeholder
            AddAscii(bodyText.Substring(idx, ph - idx));

            // Token lesen
            int end = bodyText.IndexOf("__", ph + 6, StringComparison.Ordinal);
            string token = bodyText.Substring(ph, (end + 2) - ph); // "__IMG_12__"
            string idStr = token.Replace("__IMG_", "").Replace("__", "");
            int id = int.Parse(idStr);

            // echte Bytes einfügen
            if (imageDataByObjectId.TryGetValue(id, out var bytes))
                finalBytes.AddRange(bytes);

            idx = ph + token.Length;
        }

        // startxref Offset
        long startXref = finalBytes.Count;

        // xref Offsets neu suchen
        int objectTotal = xref.Count;

        List<long> objOffsets = new List<long>();
        for (int id = 1; id <= objectTotal; id++)
        {
            byte[] marker = Encoding.ASCII.GetBytes($"{id} 0 obj");
            long pos = FindBytes(finalBytes, marker);
            objOffsets.Add(pos >= 0 ? pos : 0);
        }

        // xref schreiben
        AddAscii("xref\n");
        AddAscii($"0 {objectTotal + 1}\n");
        AddAscii("0000000000 65535 f \n");
        for (int i = 0; i < objOffsets.Count; i++)
        {
            AddAscii(objOffsets[i].ToString("0000000000") + " 00000 n \n");
        }

        // trailer
        AddAscii("trailer\n");
        AddAscii($"<< /Size {objectTotal + 1} /Root {catalogId} 0 R >>\n");
        AddAscii("startxref\n");
        AddAscii(startXref + "\n");
        AddAscii("%%EOF");

        return finalBytes.ToArray();
    }

    private static long FindBytes(List<byte> haystack, byte[] needle)
    {
        for (int i = 0; i <= haystack.Count - needle.Length; i++)
        {
            bool ok = true;
            for (int j = 0; j < needle.Length; j++)
            {
                if (haystack[i + j] != needle[j]) { ok = false; break; }
            }
            if (ok) return i;
        }
        return -1;
    }

    private int NewObject()
    {
        xref.Add(body.Length);
        return xref.Count;
    }
}
