using UnityEngine;
using System.Text;

public class TUVPreviewBuilder : MonoBehaviour
{
    public TUVTemplatePro tuv;

    public string BuildPreview(string rawText)
    {
        string clean = tuv.CleanTranscript(rawText);

        string hersteller = tuv.DetectHersteller(clean);
        string modell = tuv.DetectModell(clean);
        string baujahr = tuv.DetectBaujahr(clean);
        string kilometer = tuv.DetectKilometer(clean);
        string kennzeichen = tuv.DetectKennzeichen(clean);
        string vin = tuv.DetectVIN(clean);

        string beschreibung = tuv.DetectBeschreibung(clean);
        string bemerkungen = tuv.DetectBemerkungen(clean);

        StringBuilder sb = new StringBuilder();

        sb.AppendLine("Fahrzeugdaten");
        sb.AppendLine($"Hersteller: {hersteller}");
        sb.AppendLine($"Modell: {modell}");
        sb.AppendLine($"Baujahr: {baujahr}");
        sb.AppendLine($"Kilometerstand: {kilometer}");
        sb.AppendLine($"Kennzeichen: {kennzeichen}");
        sb.AppendLine($"VIN: {vin}");
        sb.AppendLine();
        sb.AppendLine("Beschreibung");
        sb.AppendLine(beschreibung);
        sb.AppendLine();
        sb.AppendLine("Bemerkungen");
        sb.AppendLine(bemerkungen);

        return sb.ToString();
    }
}

