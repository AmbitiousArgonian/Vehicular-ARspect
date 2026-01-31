using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class WorkflowTextLoader : MonoBehaviour
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

    public void ShowStep(int index)
    {
        currentIndex = Mathf.Clamp(index, 0, workflowList.workflow.Length - 1);
        WorkflowData w = workflowList.workflow[currentIndex];

        textField.text =
            $"<b>Workflowstep {currentIndex + 1}</b>\n\n" +
            $"<b>{w.title}</b>\n\n" +
            $"• {w.point1}\n" +
            $"• {w.point2}\n" +
            $"• {w.point3}";
    }

    public void NextWorkflow()
    {
        int next = (currentIndex + 1) % workflowList.workflow.Length;
        ShowStep(next);
    }
}
