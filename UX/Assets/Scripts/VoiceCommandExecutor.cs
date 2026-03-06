//Steuert die Workflow

using UnityEngine;

public class VoiceCommandExecutor : MonoBehaviour
{
    public VRStepController stepController;
    public TextToDocu textToDoc;

    public void Execute(string cmd)
    {
        cmd = cmd.ToLower();

        if (cmd.Contains("start"))
        {
            stepController.ShowStep(0);
            return;
        }

        if (cmd.Contains("weiter"))
        {
            stepController.NextStep();
            return;
        }

        if (cmd.Contains("zurück") || cmd.Contains("zurueck"))
        {
            stepController.PreviousStep();
            return;
        }

        if (cmd.Contains("abbrechen"))
        {
            stepController.Cancel();
            return;
        }

        if (cmd.Contains("speichern"))
        {
            textToDoc.SaveText(textToDoc.docText);
            return;
        }
    }
}




