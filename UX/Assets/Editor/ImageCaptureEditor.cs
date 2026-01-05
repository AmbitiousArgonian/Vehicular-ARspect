using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ImageCapture))]
public class ImageCaptureEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ImageCapture script = (ImageCapture)target;
        GUILayout.Space(6);
        if (GUILayout.Button("Test Capture"))
        {
            if (Application.isPlaying)
                script.TestCapture();
            else
                Debug.Log("Enter Play Mode to test capture.");
        }
    }
}
#endif
