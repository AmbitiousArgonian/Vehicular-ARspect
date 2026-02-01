using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class VehicleTextLoader : MonoBehaviour
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

     void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One)) // A
        {
            ShowVehicle(currentVehicleIndex + 1);
        }
    }

    IEnumerator SwitchVehicle()
    {
        yield return FadeText(0);
        int next = (currentVehicleIndex + 1) % vehicleList.vehicles.Length;
        ShowVehicle(next);
        yield return FadeText(1);
    }

    IEnumerator FadeText(float target)
    {
        float start = textGroup.alpha;
        float time = 0f;

        while (time < textFadeTime)
        {
            time += Time.deltaTime;
            textGroup.alpha = Mathf.Lerp(start, target, time / textFadeTime);
            yield return null;
        }
    }

}
