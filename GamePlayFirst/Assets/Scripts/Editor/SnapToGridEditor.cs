using UnityEditor;
using UnityEngine;

public class SnapToGridEditor : EditorWindow
{
    private float gridSize = 1f;

    [MenuItem("Tools/Snap To Grid %#g")] // Ctrl+Shift+G
    public static void ShowWindow()
    {
        GetWindow<SnapToGridEditor>("Snap To Grid");
    }

    private void OnGUI()
    {
        GUILayout.Label("Snap Selected Objects To Grid", EditorStyles.boldLabel);
        gridSize = EditorGUILayout.FloatField("Grid Size", gridSize);

        if (GUILayout.Button("Snap!"))
        {
            SnapSelectedObjects();
        }
    }

    private void SnapSelectedObjects()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            Undo.RecordObject(obj.transform, "Snap To Grid");
            Vector3 pos = obj.transform.position;
            Vector3 rot = obj.transform.eulerAngles;
            Vector3 scale = obj.transform.localScale;

            obj.transform.position = new Vector3(
                Mathf.Round(pos.x / gridSize) * gridSize,
                Mathf.Round(pos.y / gridSize) * gridSize,
                Mathf.Round(pos.z / gridSize) * gridSize
            );

            obj.transform.eulerAngles = new Vector3(
                Mathf.Round(rot.x),
                Mathf.Round(rot.y),
                Mathf.Round(rot.z)
            );

            obj.transform.localScale = new Vector3(
                Mathf.Round(scale.x / gridSize) * gridSize,
                Mathf.Round(scale.y / gridSize) * gridSize,
                Mathf.Round(scale.z / gridSize) * gridSize
            );
        }
    }
}
