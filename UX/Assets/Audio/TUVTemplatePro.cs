/* 

using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class TUVTemplatePro : MonoBehaviour
{
    [Header("Logo & Fotos")]
    public Texture2D tuvLogo;
    public Texture2D foto1;
    public Texture2D foto2;
    public Texture2D foto3;

    [Header("Kopfbereich")]
    public string berichtNummer = "";
    public string datum = "";
    public string prueferName = "";
    public string pruefstelle = "";
/* 
    [Header("TextToDoc Input")]
    [TextArea(10, 20)]
    public string docText;
 
    [Header("Transkript")]
    [TextArea(10, 20)]
    public string docText;

    // 🔹 NEU: Input von TextToDocu
    public void SetInput(string text, List<Texture2D> images)
    {
        docText = text;

        foto1 = images.Count > 0 ? images[0] : null;
        foto2 = images.Count > 1 ? images[1] : null;
        foto3 = images.Count > 2 ? images[2] : null;
    }
    public void CreateTUVTemplate()
    {
        // ---------------------------
        // 1) Werte aus Text extrahieren
        // ---------------------------
        string hersteller = ExtractFirst(docText, "Hersteller:");
        string modell = ExtractFirst(docText, "Modell:");
        string baujahr = ExtractFirst(docText, "Baujahr:");
        string kilometer = ExtractFirst(docText, "Kilometerstand:");
        string kennzeichen = ExtractFirst(docText, "Kennzeichen:");
        string vin = ExtractFirst(docText, "VIN:");

        string beschreibung = ExtractBlock(docText, "Pruefungsbeschreibung:");
        string bemerkungen = ExtractBlock(docText, "Bemerkungen:");

        // ---------------------------
        // 2) PDF erstellen
        // ---------------------------
        PDFDocument pdf = new PDFDocument();
        var page = pdf.AddPage();

        // ---------------------------
        // KOPFZEILE
        // ---------------------------
        if (tuvLogo != null)
            page.AddImage(tuvLogo, 40, 780, 50, 50);

        page.AddText("TECHNISCHER BERICHT", 200, 800, 22);

        page.AddText($"Bericht-Nr.: {berichtNummer}", 50, 760, 14);
        page.AddText($"Datum: {datum}", 300, 760, 14);

        page.AddText($"Pruefer: {prueferName}", 50, 740, 14);
        page.AddText($"Pruefstelle: {pruefstelle}", 50, 720, 14);

        // ---------------------------
        // FAHRZEUGDATEN
        // ---------------------------
        page.AddText("Fahrzeugdaten", 50, 690, 16);

        float y = 660;
        float step = 20;

        page.AddText($"Hersteller: {hersteller}", 50, y, 14); y -= step;
        page.AddText($"Modell: {modell}", 50, y, 14); y -= step;
        page.AddText($"Baujahr: {baujahr}", 50, y, 14); y -= step;
        page.AddText($"Kilometerstand: {kilometer}", 50, y, 14); y -= step;
        page.AddText($"Kennzeichen: {kennzeichen}", 50, y, 14); y -= step;
        page.AddText($"VIN: {vin}", 50, y, 14); y -= step;

        // ---------------------------
        // PRÜFUNGSBESCHREIBUNG
        // ---------------------------
        page.AddText("Pruefungsbeschreibung", 50, y - 10, 16);
        page.AddWrappedText(beschreibung, 50, y - 30, 500, 14);

        y -= 180;

        // ---------------------------
        // BEMERKUNGEN
        // ---------------------------
        page.AddText("Bemerkungen / Hinweise", 50, y, 16);
        page.AddWrappedText(bemerkungen, 50, y - 20, 500, 14);

        y -= 150;

        // ---------------------------
        // FOTOS
        // ---------------------------
        page.AddText("Fotos", 50, y, 16);

        if (foto1 != null) page.AddImage(foto1, 50, y - 160, 150, 150);
        if (foto2 != null) page.AddImage(foto2, 220, y - 160, 150, 150);
        if (foto3 != null) page.AddImage(foto3, 390, y - 160, 150, 150);

        // ---------------------------
        // UNTERSCHRIFT
        // ---------------------------
        page.AddText("Unterschrift Pruefer:", 50, 60, 14);
        page.AddText("__________________________", 200, 60, 14);

        // ---------------------------
        // SPEICHERN
        // ---------------------------
        byte[] data = pdf.Save();
        string path = Path.Combine(Application.persistentDataPath, "TUV_Template_Pro.pdf");
        File.WriteAllBytes(path, data);

        Debug.Log("PDF gespeichert unter: " + path);
    }

    // -------------------------------------------------
    // PARSER – ROBUST, NUR ERSTER TREFFER
    // -------------------------------------------------

    private string CleanTranscript(string text)
    {
        if (string.IsNullOrEmpty(text)) return "";

        string[] keys = {
            "Hersteller:", "Modell:", "Baujahr:", "Kilometerstand:",
            "Kennzeichen:", "VIN:", "Pruefungsbeschreibung:", "Bemerkungen:"
        };

        // Jeden Key in neue Zeile setzen
        foreach (var key in keys)
        {
            text = text.Replace(key, "\n" + key);
        }

        // Doppelte Leerzeichen entfernen
        text = System.Text.RegularExpressions.Regex.Replace(text, @"\s+", " ");

        // Zeilenumbrüche vereinheitlichen
        text = text.Replace("\r\n", "\n").Replace("\r", "\n");

        return text.Trim();
    }

    private string ExtractFirst(string text, string key)
    {
        text = CleanTranscript(text);

        int index = text.IndexOf(key, System.StringComparison.OrdinalIgnoreCase);
        if (index == -1) return "";

        index += key.Length;

        int end = text.IndexOf('\n', index);
        if (end == -1) end = text.Length;

        string value = text.Substring(index, end - index).Trim();

        // Falls der Wert weitere Keys enthält → abschneiden
        string[] stopKeys = {
            "Hersteller:", "Modell:", "Baujahr:", "Kilometerstand:",
            "Kennzeichen:", "VIN:", "Pruefungsbeschreibung:", "Bemerkungen:"
        };

        foreach (var stop in stopKeys)
        {
            int s = value.IndexOf(stop, System.StringComparison.OrdinalIgnoreCase);
            if (s > 0)
            {
                value = value.Substring(0, s).Trim();
            }
        }

        return value;
    }

    private string ExtractBlock(string text, string key)
    {
        text = CleanTranscript(text);

        int start = text.IndexOf(key, System.StringComparison.OrdinalIgnoreCase);
        if (start == -1) return "";

        start += key.Length;

        int end = text.IndexOf("Bemerkungen:", start, System.StringComparison.OrdinalIgnoreCase);
        if (end == -1) end = text.Length;

        return text.Substring(start, end - start).Trim();
    }
}
 */
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class TUVTemplatePro : MonoBehaviour
{
    [Header("Logo & Fotos")]
    public Texture2D tuvLogo;
    public Texture2D foto1;
    public Texture2D foto2;
    public Texture2D foto3;

    [Header("Kopfbereich")]
    public string berichtNummer = "";
    public string datum = "";
    public string prueferName = "";
    public string pruefstelle = "";

    [Header("Transkript / TextToDoc")]
    [TextArea(10, 20)]
    public string docText;

    // 🔹 NEU: Input von TextToDocu
    public void SetInput(string text, List<Texture2D> images)
    {
        docText = text;

        foto1 = images.Count > 0 ? images[0] : null;
        foto2 = images.Count > 1 ? images[1] : null;
        foto3 = images.Count > 2 ? images[2] : null;
    }

    public void CreateTUVTemplate()
    {   
        //prüfen, ob Bilder überhaupt im Template ankommen?
        Debug.Log($"TUVTemplatePro: foto1={(foto1!=null)} foto2={(foto2!=null)} foto3={(foto3!=null)}");

        string clean = CleanTranscript(docText);

        // AUTOMATISCHE ERKENNUNG
        string hersteller = DetectHersteller(clean);
        string modell = DetectModell(clean);
        string baujahr = DetectBaujahr(clean);
        string kilometer = DetectKilometer(clean);
        string kennzeichen = DetectKennzeichen(clean);
        string vin = DetectVIN(clean);

        string beschreibung = DetectBeschreibung(clean);
        string bemerkungen = DetectBemerkungen(clean);

        // PDF
        PDFDocument pdf = new PDFDocument();
        var page = pdf.AddPage();

        // ---------------------------
        // KOPFZEILE
        // ---------------------------
        if (tuvLogo != null)
            page.AddImage(tuvLogo, 40, 780, 50, 50);

        page.AddText("TECHNISCHER BERICHT", 200, 800, 22);
        page.AddText($"Bericht-Nr.: {berichtNummer}", 50, 760, 14);
        page.AddText($"Datum: {datum}", 300, 760, 14);
        page.AddText($"Pruefer: {prueferName}", 50, 740, 14);
        page.AddText($"Pruefstelle: {pruefstelle}", 50, 720, 14);

        // ---------------------------
        // FAHRZEUGDATEN
        // ---------------------------
        page.AddText("Fahrzeugdaten", 50, 690, 16);
        float y = 660;
        float step = 20;
        page.AddText($"Hersteller: {hersteller}", 50, y, 14); y -= step;
        page.AddText($"Modell: {modell}", 50, y, 14); y -= step;
        page.AddText($"Baujahr: {baujahr}", 50, y, 14); y -= step;
        page.AddText($"Kilometerstand: {kilometer}", 50, y, 14); y -= step;
        page.AddText($"Kennzeichen: {kennzeichen}", 50, y, 14); y -= step;
        page.AddText($"VIN: {vin}", 50, y, 14); y -= step;

        // ---------------------------
        // BESCHREIBUNG
        // ---------------------------
        page.AddText("Pruefungsbeschreibung", 50, y - 10, 16);
        page.AddWrappedText(beschreibung, 50, y - 30, 500, 14);
        y -= 180;

        // ---------------------------
        // BEMERKUNGEN
        // ---------------------------
        page.AddText("Bemerkungen / Hinweise", 50, y, 16);
        page.AddWrappedText(bemerkungen, 50, y - 20, 500, 14);
        y -= 150;

        // ---------------------------
        // FOTOS
        // ---------------------------
        page.AddText("Fotos", 50, y, 16);
        if (foto1 != null) page.AddImage(foto1, 50, y - 160, 150, 150);
        if (foto2 != null) page.AddImage(foto2, 220, y - 160, 150, 150);
        if (foto3 != null) page.AddImage(foto3, 390, y - 160, 150, 150);


 
        // ---------------------------
        // UNTERSCHRIFT
        // ---------------------------
        page.AddText("Unterschrift Pruefer:", 50, 60, 14);
        page.AddText("__________________________", 200, 60, 14);

        // ---------------------------
        // SPEICHERN
        // ---------------------------
        byte[] data = pdf.Save();
        string path = Path.Combine(Application.persistentDataPath, "TUV_Template_Pro.pdf");
        File.WriteAllBytes(path, data);

        Debug.Log("✅ PDF gespeichert unter: " + path);
    }

    // -------------------------------------------------
    // INTELLIGENTER PARSER
    // -------------------------------------------------

    private string CleanTranscript(string text)
    {
        if (string.IsNullOrEmpty(text)) return "";
        text = text.Replace("\r\n", " ").Replace("\n", " ");
        text = Regex.Replace(text, @"\s+", " ");
        return text.Trim();
    }

    private string DetectHersteller(string text)
    {
        string[] herstellerListe = { "Audi","BMW","Mercedes","Volkswagen","VW","Opel","Ford","Toyota","Hyundai","Kia","Renault","Peugeot","Seat","Skoda" };
        foreach (var h in herstellerListe)
            if (text.IndexOf(h, System.StringComparison.OrdinalIgnoreCase) >= 0)
                return h;
        return "";
    }

    private string DetectModell(string text)
    {
        var match = Regex.Match(text, @"\b[A-Z]{1,2}\d{1,3}\b");
        if (match.Success) return match.Value;

        string[] modelle = { "Golf", "Polo", "Passat", "Astra", "Focus", "Clio", "A4", "A6", "X5" };
        foreach (var m in modelle)
            if (text.IndexOf(m, System.StringComparison.OrdinalIgnoreCase) >= 0)
                return m;

        return "";
    }

    private string DetectBaujahr(string text)
    {
        var match = Regex.Match(text, @"\b(19|20)\d{2}\b");
        return match.Success ? match.Value : "";
    }

    private string DetectKilometer(string text)
    {
        var match = Regex.Match(text, @"\b\d{1,3}(\.\d{3})*\s*km\b", RegexOptions.IgnoreCase);
        return match.Success ? match.Value : "";
    }

    private string DetectKennzeichen(string text)
    {
        var match = Regex.Match(text, @"\b[A-Z]{1,3}-[A-Z]{1,2}\s*\d{1,4}\b");
        return match.Success ? match.Value : "";
    }

    private string DetectVIN(string text)
    {
        var match = Regex.Match(text, @"\b[A-HJ-NPR-Z0-9]{17}\b");
        return match.Success ? match.Value : "";
    }

    private string DetectBeschreibung(string text)
    {
        text = CleanTranscript(text);

        int start = text.IndexOf("Beschreibung", System.StringComparison.OrdinalIgnoreCase);
        if (start == -1) start = text.IndexOf("zur Beschreibung", System.StringComparison.OrdinalIgnoreCase);
        if (start == -1) return "";

        start = text.IndexOf(":", start);
        if (start == -1) start = text.IndexOf(" ", start);
        if (start == -1) return "";
        start++;

        int end = text.IndexOf("Bemerkungen", System.StringComparison.OrdinalIgnoreCase);
        if (end == -1) end = text.Length;

        string beschreibung = text.Substring(start, end - start).Trim();

        // Befehle entfernen
        string[] befehle = { "weiter", "bild aufnehmen", "foto", "jetzt", "okay", "so", "moment" };
        foreach (var b in befehle)
            beschreibung = Regex.Replace(beschreibung, @"\b" + b + @"\b", "", RegexOptions.IgnoreCase);

        // Aufräumen
        beschreibung = Regex.Replace(beschreibung, @"\s+", " ").Trim();

        return beschreibung;
    }

    private string DetectBemerkungen(string text)
    {
        return "";
    }

}
