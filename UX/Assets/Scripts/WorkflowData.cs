[System.Serializable]
public class WorkflowData
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
