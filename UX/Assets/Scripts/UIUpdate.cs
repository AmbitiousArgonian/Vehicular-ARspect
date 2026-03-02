using System.Security.Cryptography;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [Header("Right Panel")]
    public UIFader rightFader;
    public VehicleTextLoader rightLoader;

    [Header("Left Panel")]
    public UIFader leftFader;
    public WorkflowTextLoader leftLoader;

    enum UIMode { None, Vehicle, Workflow }
    UIMode currentMode = UIMode.Vehicle;

    void Start()
    {
        ShowVehicle();
    }

    void Update()
    {
        // Right Panel Vehicles 
        if (OVRInput.GetDown(OVRInput.Button.Two)) // B
        {
            ToggleVehiclePanel();
        }

        if ((currentMode == UIMode.Vehicle) && (OVRInput.GetDown(OVRInput.Button.One))) // A und Vehicle Panel geöffnet
        {
            rightLoader.NextVehicle();
        }

        // Left Panel Workflow 
        if (OVRInput.GetDown(OVRInput.Button.Four)) // Y
        {
             ToggleWorkflowPanel();;
        }

        if ((currentMode == UIMode.Workflow) && (OVRInput.GetDown(OVRInput.Button.Three))) // X und Workflow Panel geöffnet
        {
            leftLoader.NextWorkflow();
        }
    }
    void ToggleVehiclePanel()
    {
        if (currentMode == UIMode.Vehicle)
            HideAll();
        else
            ShowVehicle();
    }

    void ToggleWorkflowPanel()
    {
        if (currentMode == UIMode.Workflow)
            HideAll();
        else
            ShowWorkflow();
    }

    void ShowVehicle()
    {
        currentMode = UIMode.Vehicle;
        rightFader.FadeIn();
        leftFader.FadeOut();
    }

    void ShowWorkflow()
    {
        currentMode = UIMode.Workflow;
        leftFader.FadeIn();
        rightFader.FadeOut();
    }

    void HideAll()
    {
        currentMode = UIMode.None;
        rightFader.FadeOut();
        leftFader.FadeOut();
    }
}
