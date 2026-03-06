using UnityEngine;
using TMPro;

public class TUVStepPreviewFiller : MonoBehaviour
{
    public TextMeshProUGUI step1Text;
    public TextMeshProUGUI step2Text;
    public TextMeshProUGUI step3Text;

   public void FillSteps(string preview)
{
    string fahrzeugdaten = "";
    string beschreibung = "";
    string bemerkungen = "";

    // 1. Beschreibung finden (alle Varianten)
    string[] beschreibungsMarker = new string[]
    {
        "Beschreibung",
        "Beschreibung:",
        "Zur technischen Beschreibung",
        "Zur technischen Beschreibung:"
    };

    string[] split1 = preview.Split(beschreibungsMarker, System.StringSplitOptions.None);

    if (split1.Length > 0)
        fahrzeugdaten = split1[0].Trim();

    // 2. Bemerkungen finden (alle Varianten)
    if (split1.Length > 1)
    {
        string[] bemerkungsMarker = new string[]
        {
            "Bemerkungen",
            "Bemerkungen:",
            "Bemerkung",
            "Bemerkung:"
        };

        string[] split2 = split1[1].Split(bemerkungsMarker, System.StringSplitOptions.None);

        if (split2.Length > 0)
            beschreibung = split2[0].Trim();

        if (split2.Length > 1)
            bemerkungen = split2[1].Trim();
    }

    // Panels füllen
    step1Text.text = fahrzeugdaten;
    step2Text.text = "Beschreibung\n" + beschreibung;
    step3Text.text = "Bemerkungen\n" + bemerkungen;
}

}
