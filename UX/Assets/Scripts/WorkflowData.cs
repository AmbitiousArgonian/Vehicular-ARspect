[System.Serializable]
public class WorkflowData

/// Definiert die Struktur für einzelne Workflow-Schritte (`WorkflowData`).
/// Definiert die Struktur für eine Sammlung von Workflow-Schritten (`WorkflowList`).
///
/// Inputs: Keine, dienen als Datenmodell.
///
/// Outputs: Keine.
{
    public string title;
    public string point1;
    public string point2;
    public string point3;
}

[System.Serializable]
public class WorkflowList
{
    public WorkflowData[] workflow;
}
