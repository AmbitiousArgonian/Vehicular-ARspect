using UnityEngine;

public class VRStepController : MonoBehaviour
{
    public GameObject[] steps;   // Panels
    private int currentStep = 0;

    void Start()
    {
        ShowStep(0);
    }

    public void NextStep()
    {
        if (currentStep < steps.Length - 1)
        {
            currentStep++;
            ShowStep(currentStep);
        }
    }

    public void PreviousStep()
    {
        if (currentStep > 0)
        {
            currentStep--;
            ShowStep(currentStep);
        }
    }

    private void ShowStep(int index)
    {
        for (int i = 0; i < steps.Length; i++)
            steps[i].SetActive(i == index);
    }

    public void Save()
    {
        Debug.Log("Speichern gedrückt!");
        // Hier kommt dein Save-Code rein
    }

    public void Cancel()
    {
        Debug.Log("Nicht speichern gedrückt!");
        // Popup schließen oder zurück zur Übersicht
    }
}

