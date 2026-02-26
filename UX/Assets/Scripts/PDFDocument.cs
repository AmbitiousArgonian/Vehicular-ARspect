//Erstellt das pdf
using System.Collections.Generic;
using System.Text;

public class PDFDocument
{
    private List<PDFPage> pages = new List<PDFPage>();
    private int objectCount = 1;

    public PDFPage AddPage()
    {
        var page = new PDFPage(this);
        pages.Add(page);
        return page;
    }

    internal int GetNextObjectId()
    {
        return objectCount++;
    }

    public byte[] Save()
    {
        var writer = new PDFWriter();

        writer.StartDocument();

        List<int> pageObjectIds = new List<int>();

        foreach (var page in pages)
        {
            int pageId = writer.WritePage(page);
            pageObjectIds.Add(pageId);
        }

        int pagesId = writer.WritePages(pageObjectIds);
        int catalogId = writer.WriteCatalog(pagesId);

        return writer.EndDocument(catalogId);
    }
}

