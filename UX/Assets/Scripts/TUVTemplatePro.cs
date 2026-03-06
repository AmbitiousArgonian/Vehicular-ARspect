

using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;

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

    [Header("TextToDoc Input")]
    [TextArea(10, 20)]
    public string docText;

    public void CreateTUVTemplate()
    {
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

        // KOPFZEILE
        if (tuvLogo != null)
            page.AddImage(tuvLogo, 40, 780, 50, 50);

        page.AddText("TECHNISCHER BERICHT", 200, 800, 22);

        page.AddText($"Bericht-Nr.: {berichtNummer}", 50, 760, 14);
        page.AddText($"Datum: {datum}", 300, 760, 14);

        page.AddText($"Pruefer: {prueferName}", 50, 740, 14);
        page.AddText($"Pruefstelle: {pruefstelle}", 50, 720, 14);

        // FAHRZEUGDATEN
        page.AddText("Fahrzeugdaten", 50, 690, 16);

        float y = 660;
        float step = 20;

        page.AddText($"Hersteller: {hersteller}", 50, y, 14); y -= step;
        page.AddText($"Modell: {modell}", 50, y, 14); y -= step;
        page.AddText($"Baujahr: {baujahr}", 50, y, 14); y -= step;
        page.AddText($"Kilometerstand: {kilometer}", 50, y, 14); y -= step;
        page.AddText($"Kennzeichen: {kennzeichen}", 50, y, 14); y -= step;
        page.AddText($"VIN: {vin}", 50, y, 14); y -= step;

        // BESCHREIBUNG
        page.AddText("Pruefungsbeschreibung", 50, y - 10, 16);
        page.AddWrappedText(beschreibung, 50, y - 30, 500, 14);

        y -= 300;

        
       // BEMERKUNGEN
    page.AddText("Bemerkungen / Hinweise", 50, y, 16);
    if (!string.IsNullOrWhiteSpace(bemerkungen))
    {
 
    bemerkungen = " * " + bemerkungen;
    }

page.AddWrappedText(bemerkungen, 50, y - 20, 500, 14);


        y -= 200;

        // FOTOS
        page.AddText("Fotos", 50, y, 16);

        if (foto1 != null) page.AddImage(foto1, 50, y - 160, 150, 150);
        if (foto2 != null) page.AddImage(foto2, 220, y - 160, 150, 150);
        if (foto3 != null) page.AddImage(foto3, 390, y - 160, 150, 150);

        // UNTERSCHRIFT
        page.AddText("Unterschrift Pruefer:", 50, 60, 14);
        page.AddText("__________________________", 200, 60, 14);

        // SPEICHERN – NEUE VERSION
        Debug.Log("NEUE PDF-SPEICHERLOGIK WIRD AUSGEFÜHRT!");
byte[] data = pdf.Save();
Debug.Log("PDF-Bytes erzeugt: " + data.Length);


// Ordner erstellen
string reportsFolder = Path.Combine(Application.persistentDataPath, "Reports");
Directory.CreateDirectory(reportsFolder);
Debug.Log("Reports-Ordner: " + reportsFolder);

// Dateiname mit Zeitstempel
string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
string filename = $"TUV_Report_{timestamp}.pdf";
string fullPath = Path.Combine(reportsFolder, filename);

// PDF speichern
File.WriteAllBytes(fullPath, data);
Debug.Log("PDF gespeichert unter: " + fullPath);

// JSON-Index aktualisieren
UpdateReportIndex(filename, fullPath, timestamp);

    }

    // -------------------------------------------------
    // INTELLIGENTER PARSER
    // -------------------------------------------------

    public string CleanTranscript(string text)
    {
        if (string.IsNullOrEmpty(text)) return "";

        text = text.Replace("\r\n", " ").Replace("\n", " ");
        text = Regex.Replace(text, @"\s+", " ");

        return text.Trim();
    }

    public string DetectHersteller(string text)
    {
        string[] herstellerListe = {
            "Audi","BMW","Mercedes","Volkswagen","VW","Opel","Ford",
            "Toyota","Hyundai","Kia","Renault","Peugeot","Seat","Skoda"
        };

        foreach (var h in herstellerListe)
            if (text.IndexOf(h, System.StringComparison.OrdinalIgnoreCase) >= 0)
                return h;

        return "";
    }

    public string DetectModell(string text)
    {
        var match = Regex.Match(text, @"\b[A-Z]{1,2}\d{1,3}\b");
        if (match.Success) return match.Value;

        string[] modelle = { "Golf", "Polo", "Passat", "Astra", "Focus", "Clio" };
        foreach (var m in modelle)
            if (text.IndexOf(m, System.StringComparison.OrdinalIgnoreCase) >= 0)
                return m;

        return "";
    }

    public string DetectBaujahr(string text)
    {
        var match = Regex.Match(text, @"\b(19|20)\d{2}\b");
        return match.Success ? match.Value : "";
    }

    public string DetectKilometer(string text)
    {
        var match = Regex.Match(text, @"\b\d{1,3}(\.\d{3})*\s*km\b", RegexOptions.IgnoreCase);
        return match.Success ? match.Value : "";
    }

    public string DetectKennzeichen(string text)
    {
        var match = Regex.Match(text, @"\b[A-Z]{1,3}-[A-Z]{1,2}\s*\d{1,4}\b");
        return match.Success ? match.Value : "";
    }

    public string DetectVIN(string text)
    {
        var match = Regex.Match(text, @"\b[A-HJ-NPR-Z0-9]{17}\b");
        return match.Success ? match.Value : "";
    }

    public string DetectBeschreibung(string text)
    {
        text = CleanTranscript(text);

        // Beschreibung Marker
        string[] beschreibungsMarker = {
            "Beschreibung",
            "Beschreibung:",
            "Zur technischen Beschreibung",
            "Zur technischen Beschreibung:"
        };

        int start = -1;

        foreach (var m in beschreibungsMarker)
        {
            int idx = text.IndexOf(m, System.StringComparison.OrdinalIgnoreCase);
            if (idx != -1)
            {
                start = idx + m.Length;
                break;
            }
        }

        if (start == -1)
            return "";

        // Bemerkungs Marker
        string[] bemerkungsMarker = {
            "Bemerkungen",
            "Bemerkungen:",
            "Bemerkung",
            "Bemerkung:",
            "Hinweise",
            "Hinweise:",
            "Anmerkungen",
            "Anmerkungen:"
        };

        int end = -1;

        foreach (var m in bemerkungsMarker)
        {
            int idx = text.IndexOf(m, System.StringComparison.OrdinalIgnoreCase);
            if (idx != -1)
            {
                end = idx;
                break;
            }
        }

        if (end == -1)
            end = text.Length;

        if (end <= start)
            return "";

        string beschreibung = text.Substring(start, end - start).Trim();
        beschreibung = Regex.Replace(beschreibung, @"\s+", " ").Trim();

        // hier die Stichpunkt-Analyse
        beschreibung = AnalyseBeschreibungAlsListe(beschreibung);

        return beschreibung;
    }

   public string AnalyseBeschreibungAlsListe(string beschreibung)
{
    if (string.IsNullOrWhiteSpace(beschreibung))
        return "";

    string[] saetze = beschreibung.Split('.', '!', '?');

    List<string> gut = new List<string>();
    List<string> kritisch = new List<string>();
    List<string> anmerkung = new List<string>();

    foreach (string s in saetze)
    {
        string t = s.Trim();
        if (t.Length < 3) continue;

        // --- GUT ---
        if (Regex.IsMatch(t, @"ruhig|gut|funktionsfähig|keine|stabil|einwandfrei|vollständig", RegexOptions.IgnoreCase))
        {
            gut.Add(" * " + t);
            continue;
        }

        // --- BESCHAEDIGT / KRITISCH ---
        if (Regex.IsMatch(t, @"verschlissen|beschaedigt|defekt|undicht|leck|kritisch|abgenutzt|kratzer|delle", RegexOptions.IgnoreCase))
        {
            kritisch.Add(" * " + t);
            continue;
        }

        // --- SPEZIAL: Reifenprofil ---
        if (Regex.IsMatch(t, @"\b\d+\s*mm\b", RegexOptions.IgnoreCase))
        {
            MatchCollection matches = Regex.Matches(t, @"\b(\d+)\s*mm\b");
            bool isCritical = false;

            foreach (Match m in matches)
            {
                int mm = int.Parse(m.Groups[1].Value);
                if (mm < 4) isCritical = true;
            }

            if (isCritical)
                kritisch.Add(" * " + t);
            else
                gut.Add(" * " + t);

            continue;
        }

        // --- ANMERKUNG ---
        anmerkung.Add(" * " + t);
    }

    StringBuilder sb = new StringBuilder();

    if (gut.Count > 0)
    {
        sb.AppendLine("GUT:");
        foreach (var g in gut) sb.AppendLine(g);
        sb.AppendLine();
    }

    if (kritisch.Count > 0)
    {
        sb.AppendLine("BESCHAEDIGT / KRITISCH:");
        foreach (var k in kritisch) sb.AppendLine(k);
        sb.AppendLine();
    }

    if (anmerkung.Count > 0)
    {
        sb.AppendLine("ANMERKUNGEN:");
        foreach (var a in anmerkung) sb.AppendLine(a);
    }

    return sb.ToString().Trim();
}


    public string DetectBemerkungen(string text)
    {
        text = CleanTranscript(text);

        string[] marker = {
            "Bemerkungen",
            "Bemerkungen:",
            "Bemerkung",
            "Bemerkung:",
            "Hinweise",
            "Hinweise:",
            "Anmerkungen",
            "Anmerkungen:"
        };

        int start = -1;

        foreach (var m in marker)
        {
            int idx = text.IndexOf(m, System.StringComparison.OrdinalIgnoreCase);
            if (idx != -1)
            {
                start = idx + m.Length;
                break;
            }
        }

        if (start == -1)
            return "";

        string bemerkungen = text.Substring(start).Trim();
        bemerkungen = Regex.Replace(bemerkungen, @"\s+", " ").Trim();

        return bemerkungen;
    }

  [System.Serializable]
public class ReportEntry
{
    public string id;
    public string filename;
    public string path;
    public string created;
}

[System.Serializable]
public class ReportIndex
{
    public List<ReportEntry> reports = new List<ReportEntry>();
}

private void UpdateReportIndex(string filename, string fullPath, string timestamp)
{
    string indexPath = Path.Combine(Application.persistentDataPath, "Reports", "reports.json");

    ReportIndex index;
    Debug.Log("UpdateReportIndex wurde aufgerufen!");


    // Falls JSON existiert → laden
    if (File.Exists(indexPath))
    {
        string json = File.ReadAllText(indexPath);
        index = JsonUtility.FromJson<ReportIndex>(json);
    }
    else
    {
        index = new ReportIndex();
    }

    // Neuen Eintrag hinzufügen
    index.reports.Add(new ReportEntry
    {
        id = timestamp,
        filename = filename,
        path = fullPath,
        created = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
    });

    // JSON speichern
    string newJson = JsonUtility.ToJson(index, true);
    File.WriteAllText(indexPath, newJson);

    Debug.Log("Report-Index aktualisiert: " + indexPath);
}
  
}
