using UnityEngine;

public class UIController : MonoBehaviour
{
    [Header("Right Panel")]
    public UIFader rightFader;
    public JsonTextLoader rightLoader;

    [Header("Left Panel")]
    public UIFader leftFader;
    public JsonTextLoader leftLoader;

    private bool rightVisible = true;
    private bool leftVisible = true;

    void Update()
    {
        // Right Panel Vehicles
        if (OVRInput.GetDown(OVRInput.Button.Two)) // B
        {
            rightVisible = !rightVisible;
            if (rightVisible) rightFader.FadeIn();
            else rightFader.FadeOut();
        }

        if (OVRInput.GetDown(OVRInput.Button.One)) // A
        {
            rightLoader.NextVehicle();
        }

        // Left Panel Workflow
        if (OVRInput.GetDown(OVRInput.Button.Four)) // Y
        {
            leftVisible = !leftVisible;
            if (leftVisible) leftFader.FadeIn();
            else leftFader.FadeOut();
        }

        if (OVRInput.GetDown(OVRInput.Button.Three)) // X
        {
            leftLoader.NextVehicle();
        }
    }
}
