using System.IO;
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;


public class ImageCapture : MonoBehaviour
{
    public Camera xrCamera;

    public void OnVoiceCommand(string text)
    {
        if (string.IsNullOrEmpty(text)) return;
        if (text.ToLower().Contains("bitte nimm bild auf"))
            StartCoroutine(Capture(text));
    }

    // Test‑Methode für Inspector/UI
    public void TestCapture() => OnVoiceCommand("Bitte nimm Bild auf");

    // Shortcut: Taste T auslösen (Editor & Build)
  /** #if UNITY_EDITOR
void Update()
{
    if (UnityEngine.Input.GetKeyDown(KeyCode.T))
        TestCapture();
}
#endif
*/
void Update()
{
    if (Keyboard.current != null &&
        Keyboard.current.tKey.wasPressedThisFrame)
    {
        TestCapture();
    }
}



    IEnumerator Capture(string label)
    {
        Debug.Log("Capture started");
        yield return new WaitForEndOfFrame();

        int w = Screen.width;
        int h = Screen.height;
        RenderTexture rt = new RenderTexture(w, h, 24);
        if (xrCamera == null) xrCamera = Camera.main;
        xrCamera.targetTexture = rt;
        xrCamera.Render();

        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(w, h, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, w, h), 0, 0);
        tex.Apply();

        xrCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        string safe = MakeSafe(label);
        string name = $"{safe}_{System.DateTime.Now:yyyyMMdd_HHmmss}.png";
        string path = Path.Combine(Application.persistentDataPath, name);
        File.WriteAllBytes(path, tex.EncodeToPNG());

        Debug.Log("Saved image: " + path);
    }

    string MakeSafe(string s)
    {
        foreach (var c in Path.GetInvalidFileNameChars())
            s = s.Replace(c, '_');
        return s.Replace(" ", "_");
    }
}
