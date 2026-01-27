using UnityEngine;

public class UIVisibilityToggle : MonoBehaviour
{
    public GameObject panelObject;      
    public OVROverlay overlay;          // Panel
    public JsonTextLoader loader;       // JSON Script   
    private bool isVisible = true;

    void Update()
    {
        // Set here trigger for visibility of panel rn B-Button
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            isVisible = !isVisible;
            panelObject.SetActive(isVisible);
            if (overlay != null)
                overlay.enabled = isVisible;
        }
        // Set here trigger for visibility of panel rn A-Button
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            if (loader != null)
                loader.NextVehicle();
        }
    }
}
