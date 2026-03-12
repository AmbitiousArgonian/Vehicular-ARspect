using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class VehicleTextLoader : MonoBehaviour
/// Summary:
/// Phrased und verarbeitet json mit Fahrzeug Informationen und bereitet aktuellen Index zur Ausgabe vor
/// 
/// - Lädt Vehicle-Daten aus Streaming assets mit Namen in Variable filename.
/// - Parst die JSON-Daten in eine `VehicleList` (Array von `VehicleData`).
/// - Zeigt einzelne Vehicle sequenziell in einem Textfeld an.
/// - Ermöglicht das Navigieren zum nächsten Workflow-Schritt.
///
/// Inputs:
/// - `textField`: Ein `TextMeshProUGUI`-Objekt, in dem der Workflow-Text angezeigt wird. Gibt Syntax in JSON vor
/// - `fileName`: Der Name der JSON-Datei, die die Workflow-Daten enthält (In Demo: "VehicleInfo.json").
///
/// Outputs:
/// - Aktualisiert das `textField` mit dem aktuellen Vehicle und Navigationsinformationen.
/// - Debug-Meldungen bei Fehlern (z.B. Datei nicht gefunden, keine Daten).
/// - Aktuell eingeblender Vehicleindex um Diktate einen Arbeitschritt zuzuordnen
{
    public TextMeshProUGUI textField;
    public string fileName = "VehicleInfo.json";
    public CanvasGroup textGroup;
    public float textFadeTime = 0.15f;


    private VehicleList vehicleList;
    private int currentVehicleIndex = 0;

    void Start()
    {
        StartCoroutine(LoadText());
    }

    IEnumerator LoadText()
    {
        string path = System.IO.Path.Combine(UnityEngine.Application.streamingAssetsPath, fileName);
        Debug.Log("PATH: " + path);

#if UNITY_ANDROID && !UNITY_EDITOR
        UnityWebRequest request = UnityWebRequest.Get(path);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("JSON LOADED");
            ProcessJson(request.downloadHandler.text);
        }
        else
        {
            Debug.Log("ERROR: " + request.error);
            textField.text = "File not found (Android).";
        }
#else
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            Debug.Log("JSON LOADED");
            ProcessJson(json);
        }
        else
        {
            Debug.Log("FILE NOT FOUND");
            textField.text = "File not found.";
        }
#endif

        yield break;
    }


    void ProcessJson(string json)
    {
        vehicleList = JsonUtility.FromJson<VehicleList>(json);

        if (vehicleList == null || vehicleList.vehicles.Length == 0)
        {
            textField.text = "No vehicle data.";
            return;
        }

        ShowVehicle(0);
    }

    public void ShowVehicle(int index)
    {
        currentVehicleIndex = index % vehicleList.vehicles.Length;
        VehicleData v = vehicleList.vehicles[currentVehicleIndex];
        int printIndex = currentVehicleIndex;
            printIndex = printIndex + 1;
        textField.text =
            $"<b>Fahrzeug {printIndex} / {vehicleList.vehicles.Length}</b>\n\n" +
            $"FIN: {v.FIN}\n" +
            $"ABEs: {v.ABEs}\n" +
            $"Defects: {v.defReport}";
    }

    public int GetCurrentVehhicleIndex()
    { return currentVehicleIndex; }


    public void NextVehicle()
    {
        ShowVehicle(currentVehicleIndex + 1);
    }

}
