/*using UnityEngine;
using TMPro;



public class VRInspectionPreview : MonoBehaviour
{
    public GameObject popupPanel;
    [Header("Datenquelle")]
    public TextToDocu textSource;

    [Header("UI")]
    public TextMeshProUGUI previewText;

    void Start()
    {
        RefreshPreview();
    }

   public TUVStepPreviewFiller stepPreviewFiller;

public void RefreshPreview()
{
    var builder = FindObjectOfType<TUVPreviewBuilder>();
    string analyzed = builder.BuildPreview(textSource.docText);

    stepPreviewFiller.FillSteps(analyzed);
}

    public void OnSavePressed()
    {
        Debug.Log("VR: Speichern gedrückt");

        var builder = FindObjectOfType<TUVPreviewBuilder>();
        string analyzed = builder.BuildPreview(textSource.docText);

        textSource.SaveText(analyzed);
    }

    public void OnEditPressed()
    {
        Debug.Log("VR: Bearbeiten gedrückt");
        popupPanel.SetActive(true);
    }

    public void OnPopupYes()
{
    // Speichern wie beim Save-Button
    var builder = FindObjectOfType<TUVPreviewBuilder>();
    string analyzed = builder.BuildPreview(textSource.docText);
    textSource.SaveText(analyzed);

    popupPanel.SetActive(false);
}

public void OnPopupNo()
{
    // Popup einfach schließen
    popupPanel.SetActive(false);
}

}

*/

using UnityEngine;

public class VRInspectionPreview : MonoBehaviour
{
    public TextToDocu textSource;
    public TUVStepPreviewFiller stepPreviewFiller;

    void Start()
    {
        RefreshPreview();
    }

    public void RefreshPreview()
    {
        var builder = FindObjectOfType<TUVPreviewBuilder>();
        string analyzed = builder.BuildPreview(textSource.docText);

        // Text in Step1/2/3 Panels einfügen
        stepPreviewFiller.FillSteps(analyzed);
    }

    public void OnSavePressed()
    {
        var builder = FindObjectOfType<TUVPreviewBuilder>();
        string analyzed = builder.BuildPreview(textSource.docText);

        // Speichern in Template (alte Logik)
        textSource.SaveText(analyzed);
    }
}

