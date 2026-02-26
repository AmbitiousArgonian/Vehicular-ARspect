
/*
using UnityEngine;

public class TextToDocu : MonoBehaviour
{
    [TextArea(8, 20)]
    public string docText;   // zeigt den Transkripttext im Inspector

    // Wird vom Audio-System aufgerufen:
    // SaveText(transcriptionString)
    public void SaveText(string newText)
    {
        Debug.Log("SaveText wurde aufgerufen!");

        // Text speichern
        docText = newText ?? "";

        // TUVTemplatePro suchen
        TUVTemplatePro tuv = FindObjectOfType<TUVTemplatePro>();

        if (tuv == null)
        {
            Debug.LogError("TUVTemplatePro wurde NICHT gefunden!");
            return;
        }

        // Text an die Vorlage übergeben
        tuv.docText = docText;

        // PDF erzeugen
        tuv.CreateTUVTemplate();

        Debug.Log("Transkript gespeichert und an TUVTemplatePro übergeben.");
    }
}
*/

using UnityEngine;

public class TextToDocu : MonoBehaviour
{
    [TextArea(8, 20)]
    public string docText;

    public void SaveText(string newText)
    {
        Debug.Log("SaveText wurde aufgerufen!");

        docText = newText ?? "";

        TUVTemplatePro tuv = FindObjectOfType<TUVTemplatePro>();

        if (tuv == null)
        {
            Debug.LogError("TUVTemplatePro wurde NICHT gefunden!");
            return;
        }

        tuv.docText = docText;
        tuv.CreateTUVTemplate();

        Debug.Log("Transkript gespeichert und an TUVTemplatePro übergeben.");
    }
}
