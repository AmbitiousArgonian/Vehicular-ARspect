using System.Security.Cryptography;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [Header("Right Panel")]
    public UIFader rightFader;
    public JsonTextLoader rightLoader;

    [Header("Left Panel")]
    public UIFader leftFader;
    public JsonTextLoader leftLoader;

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

        if (OVRInput.GetDown(OVRInput.Button.One)) // A
        {
            rightLoader.NextVehicle();
        }

        // Left Panel Workflow
        if (OVRInput.GetDown(OVRInput.Button.Four)) // Y
        {
             ToggleWorkflowPanel();;
        }

        if (OVRInput.GetDown(OVRInput.Button.Three)) // X
        {
            leftLoader.NextVehicle();
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
