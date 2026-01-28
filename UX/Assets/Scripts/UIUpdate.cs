using UnityEngine;

public class UIVisibilityToggle : MonoBehaviour
{
    public UIFader fader;
    public GameObject panelObject;
    public JsonTextLoader loader;  
    private bool isVisible = true;

    void Update()
    {
        //  Visibility
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            isVisible = !isVisible;
            if (isVisible)
                fader.FadeIn();
            else
                fader.FadeOut();
        }
        // Next Vehicle
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            loader.NextVehicle();
        }
    }
}
