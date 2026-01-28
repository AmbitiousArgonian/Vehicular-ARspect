using UnityEngine;

public class UIVisibilityToggle : MonoBehaviour
{
    public GameObject panelObject;
    public JsonTextLoader loader;  
    private bool isVisible = true;

    void Update()
    {
        //  Visibility
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            isVisible = !isVisible;
            panelObject.SetActive(isVisible);
        }
        // Next Vehicle
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            loader.NextVehicle();
        }
    }
}
