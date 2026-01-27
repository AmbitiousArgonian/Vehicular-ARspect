using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class JsonTextLoader : MonoBehaviour
{
    public TextMeshProUGUI textField;
    public string fileName = "VehicleInfo.json";

    void Start()
    {
        StartCoroutine(LoadText());
    }

    IEnumerator LoadText()
    {
        string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, fileName);

#if UNITY_ANDROID && !UNITY_EDITOR
    UnityWebRequest request = UnityWebRequest.Get(filePath);
    yield return request.SendWebRequest();

    if (request.result != UnityWebRequest.Result.Success)
    {
        Debug.LogError("LOAD ERROR: " + request.error);
        textField.text = "Load error:\n" + request.error;
    }
    else
    {
        Debug.Log("JSON RAW:\n" + request.downloadHandler.text);
        textField.text = request.downloadHandler.text;

    }
#else
        if (System.IO.File.Exists(filePath))
        {
            string json = System.IO.File.ReadAllText(filePath);
            Debug.Log("JSON RAW:\n" + json);
            ProcessJson(json);
        }
        else
        {
            textField.text = "File not found.";
        }
#endif
        yield break;
    }


    private VehicleList vehicleList;
    private int currentVehicleIndex = 0;

    void ProcessJson(string json)
    {
        vehicleList = JsonUtility.FromJson<VehicleList>(json);
        Debug.Log("Vehicle count: " + vehicleList?.vehicles?.Length); 


        if (vehicleList == null || vehicleList.vehicles.Length == 0)
        {
            textField.text = "No vehicle data.";
            return;
        }

        ShowVehicle(0);
    }

    public void ShowVehicle(int index)
    {
        currentVehicleIndex = Mathf.Clamp(index, 0, vehicleList.vehicles.Length - 1);
        VehicleData v = vehicleList.vehicles[currentVehicleIndex];

        textField.text =
            $"<b>Fahrzeug {currentVehicleIndex + 1}</b>\n\n" +
            $"FIN: {v.FIN}\n" +
            $"ABEs: {v.ABEs}\n" +
            $"def. Report: {v.defReport}";
    }

    public void NextVehicle()
    {
        int next = (currentVehicleIndex + 1) % vehicleList.vehicles.Length;
        ShowVehicle(next);
    }

}
