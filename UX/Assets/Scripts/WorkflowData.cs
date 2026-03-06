using System.Collections.Generic;

[System.Serializable]
public class WorkflowImage
{
    public string path;
    public string label;
    public string timestamp;
}

[System.Serializable]
public class WorkflowData
{
    public string title;
    public string point1;
    public string point2;
    public string point3;

    // Neue Liste für Bilder
    public List<WorkflowImage> images = new List<WorkflowImage>();
}

[System.Serializable]
public class WorkflowList
{
    public WorkflowData[] workflow;
}