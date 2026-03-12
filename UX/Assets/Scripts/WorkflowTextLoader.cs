using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class WorkflowTextLoader : MonoBehaviour

/// Summary:
/// Phrased und verarbeitet json mit Arbeitsablauf Informationen und bereitet aktuellen Index zur Ausgabe vor
/// 
/// - Lädt Workflow-Daten aus Streaming assets mit Namen in Variable filename.
/// - Parst die JSON-Daten in eine `WorkflowList` (Array von `WorkflowData`).
/// - Zeigt einzelne Workflow-Schritte sequenziell in einem Textfeld an.
/// - Ermöglicht das Navigieren zum nächsten Workflow-Schritt.
///
/// Inputs:
/// - `textField`: Ein `TextMeshProUGUI`-Objekt, in dem der Workflow-Text angezeigt wird. Gibt Syntax in JSON vor
/// - `fileName`: Der Name der JSON-Datei, die die Workflow-Daten enthält (In Demo: "WorkflowInfo.json").
///
/// Outputs:
/// - Aktualisiert das `textField` mit dem aktuellen Workflow-Schritt und Navigationsinformationen.
/// - Debug-Meldungen bei Fehlern (z.B. Datei nicht gefunden, keine Daten).
/// - Aktuell eingeblender Workflowindex um Diktate einen Arbeitschritt zuzuordnen
{
    public TextMeshProUGUI textField;
    public string fileName = "WorkflowInfo.json";

    private WorkflowList workflowList;
    private int currentIndex = 0;

    void Start()
    {
        StartCoroutine(LoadWorkflow());
    }

    IEnumerator LoadWorkflow()
    {
        string path = System.IO.Path.Combine(UnityEngine.Application.streamingAssetsPath, fileName);

#if UNITY_ANDROID && !UNITY_EDITOR
        UnityWebRequest request = UnityWebRequest.Get(path);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
            ProcessJson(request.downloadHandler.text);
        else
            textField.text = "Workflow file not found.";
#else
        if (System.IO.File.Exists(path))
            ProcessJson(System.IO.File.ReadAllText(path));
        else
            textField.text = "Workflow file not found.";
#endif
        yield break;
    }

    void ProcessJson(string json)
    {
        workflowList = JsonUtility.FromJson<WorkflowList>(json);

        if (workflowList == null || workflowList.workflow.Length == 0)
        {
            textField.text = "No workflow data.";
            return;
        }

        ShowStep(0);
    }

    public int GetCurrentWorkflowIndex()
        { return currentIndex; }

    public void ShowStep(int index)
    {
        currentIndex = index % workflowList.workflow.Length;
        WorkflowData w = workflowList.workflow[currentIndex];
        int printIndex = currentIndex;
        printIndex = printIndex + 1;

        textField.text =
            $"<b>Workflowstep {printIndex} / {workflowList.workflow.Length}</b>\n\n" +
            $"<b>{w.title}</b>\n" +
            $"• {w.point1}\n" +
            $"• {w.point2}\n" +
            $"• {w.point3}";
    }

    public void NextWorkflow()
    { 
        ShowStep(currentIndex + 1);
    }
}
