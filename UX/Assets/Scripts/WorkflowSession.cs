using UnityEngine;

public class WorkflowSession : MonoBehaviour
{
    public static WorkflowSession Instance;

    public WorkflowData data = new WorkflowData();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}