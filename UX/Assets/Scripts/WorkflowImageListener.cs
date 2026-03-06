using UnityEngine;
using System;

public class WorkflowImageListener : MonoBehaviour
{
    WorkflowData workflowData => WorkflowSession.Instance.data;

    private void OnEnable()
    {
        AppEvents.ImageCaptured += OnImageCaptured;
    }

    private void OnDisable()
    {
        AppEvents.ImageCaptured -= OnImageCaptured;
    }

    private void OnImageCaptured(string path, string label)
    {
        WorkflowImage img = new WorkflowImage();
        img.path = path;
        img.label = label;
        img.timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        workflowData.images.Add(img);

        Debug.Log("Bild zum Workflow hinzugefügt: " + path);
    }
}