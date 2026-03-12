//using System.Security.Cryptography;
using UnityEngine;

public class UIController : MonoBehaviour

/// Summary:
/// Kontrolliert und Steuert das UI, zentrale Anlaufstelle, Steuerelement und Initialiator
///
/// - Verwaltet die Sichtbarkeit des rechten (Fahrzeug) und linken (Workflow) UI-Panels.
/// - Reagiert auf spezifische OVRInput-Tasten, um Panels umzuschalten oder Inhalte zu wechseln.
/// - Koordiniert das Ein- und Ausblenden der Panels über `UIFader`-Komponenten.
/// - Steuert die Navigation innerhalb der `VehicleTextLoader` und `WorkflowTextLoader` Komponenten.
///
/// Inputs:
/// - `rightFader`: Die `UIFader`-Komponente für das rechte Panel.
/// - `rightLoader`: Der `VehicleTextLoader` für das rechte Panel.
/// - `leftFader`: Die `UIFader`-Komponente für das linke Panel.
/// - `leftLoader`: Der `WorkflowTextLoader` für das linke Panel.
/// - OVRInput-Tasten (B, A, Y, X) für Panel-Steuerung und Inhaltsnavigation.
///
/// Outputs:
/// - Ruft `FadeIn()` und `FadeOut()` Methoden auf den `UIFader`-Komponenten auf.
/// - Ruft `NextVehicle()` auf dem `rightLoader` und `NextWorkflow()` auf dem `leftLoader` auf.
/// - Ändert den internen `currentMode`-Status, um den aktiven Panel-Typ zu verfolgen.
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
    //Updateschleife wird jeden Frame durchlaufen und reagiert auf User Input
    {
        // Right Panel Vehicles 
        if (OVRInput.GetDown(OVRInput.Button.Two)) // B
        {
            ToggleVehiclePanel();
        }

        if ((currentMode == UIMode.Vehicle) && (OVRInput.GetDown(OVRInput.Button.One))) // Controllertaste A und Vehicle Panel geöffnet; hier würde die Sprachsteuerung "(OVRInput.GetDown(OVRInput.Button.One)" ersetzen und das Vehicle UI Element steuern
        {
            rightLoader.NextVehicle();
        }

        // Left Panel Workflow 
        if (OVRInput.GetDown(OVRInput.Button.Four)) // Y
        {
             ToggleWorkflowPanel();;
        }

        if ((currentMode == UIMode.Workflow) && (OVRInput.GetDown(OVRInput.Button.Three))) // Controllertaste X und Workflow Panel geöffnet; hier würde die Sprachsteuerung "(OVRInput.GetDown(OVRInput.Button.One)" ersetzen und das Workflow UI Element steuern
        {
            leftLoader.NextWorkflow();
        }
    }
    void ToggleVehiclePanel()
    {
        // garantiert das nur ein UI Element soll zeitgleich eingeblendet sein kann
        if (currentMode == UIMode.Vehicle)
            HideAll();
        else
            ShowVehicle();
    }

    void ToggleWorkflowPanel()
    {
        // garantiert das nur ein UI Element soll zeitgleich eingeblendet sein kann
        if (currentMode == UIMode.Workflow)
            HideAll();
        else
            ShowWorkflow();
    }

    //show Funktionen lösen Animation in UIFades aus
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
