[System.Serializable]
public class WorkflowData
{
    public string Titel;
    public string Point1;
    public string Point2;
    public string Point3;
}

[System.Serializable]
public class WorkflowList
{
    public WorkflowData[] WorkflowItem;
}