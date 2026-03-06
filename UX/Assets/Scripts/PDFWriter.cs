//Schreibt die PDF‑Struktur (Header, XRef, Streams)
using System.Collections.Generic;
using System.Text;

public class PDFWriter
{
    private StringBuilder body = new StringBuilder();
    private List<long> xref = new List<long>();

    public void StartDocument()
    {
        body.Append("%PDF-1.4\n");
    }

    public int WritePage(PDFPage page)
    {
        int pageId = NewObject();

        StringBuilder content = new StringBuilder();
        content.Append("BT\n");

        foreach (var cmd in page.TextCommands)
            content.Append(cmd + "\n");

        content.Append("ET\n");

        foreach (var img in page.Images)
        {
            int imgId = WriteImage(img);

            content.Append($"q\n{img.W} 0 0 {img.H} {img.X} {img.Y} cm\n/Im{imgId} Do\nQ\n");
        }

        int contentId = WriteStream(content.ToString());

        body.Append($"{pageId} 0 obj\n");
        body.Append("<< /Type /Page /Parent 0 0 R /Contents " + contentId + " 0 R >>\n");
        body.Append("endobj\n");

        return pageId;
    }

    public int WriteImage(PDFImage img)
    {
        int id = NewObject();

        body.Append($"{id} 0 obj\n");
        body.Append("<< /Type /XObject /Subtype /Image ");
        body.Append($"/Width {img.Width} /Height {img.Height} ");
        body.Append("/ColorSpace /DeviceRGB /BitsPerComponent 8 ");
        body.Append($"/Filter /DCTDecode /Length {img.RawData.Length} >>\n");
        body.Append("stream\n");
        body.Append(Encoding.ASCII.GetString(img.RawData));
        body.Append("\nendstream\nendobj\n");

        return id;
    }

    public int WritePages(List<int> pageIds)
    {
        int id = NewObject();

        body.Append($"{id} 0 obj\n");
        body.Append("<< /Type /Pages /Kids [");

        foreach (var pid in pageIds)
            body.Append($"{pid} 0 R ");

        body.Append($"] /Count {pageIds.Count} >>\nendobj\n");

        return id;
    }

    public int WriteCatalog(int pagesId)
    {
        int id = NewObject();

        body.Append($"{id} 0 obj\n");
        body.Append($"<< /Type /Catalog /Pages {pagesId} 0 R >>\nendobj\n");

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
        long startXref = body.Length;

        StringBuilder final = new StringBuilder();
        final.Append(body.ToString());

        final.Append("xref\n");
        final.Append($"0 {xref.Count + 1}\n");
        final.Append("0000000000 65535 f \n");

        foreach (var pos in xref)
            final.Append(pos.ToString("0000000000") + " 00000 n \n");

        final.Append("trailer\n");
        final.Append($"<< /Size {xref.Count + 1} /Root {catalogId} 0 R >>\n");
        final.Append("startxref\n");
        final.Append(startXref + "\n");
        final.Append("%%EOF");

        return Encoding.ASCII.GetBytes(final.ToString());
    }

    private int NewObject()
    {
        xref.Add(body.Length);
        return xref.Count;
    }
}
