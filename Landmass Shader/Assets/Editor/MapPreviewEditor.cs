using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapPreview))]
public class MapPreviewEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Target is the object we are inspecting
        MapPreview mapPreview = (MapPreview) target;
        if (DrawDefaultInspector())
        {
            if (mapPreview.AutoUpdate)
                mapPreview.DrawMap();
        }

        if (GUILayout.Button("Generate"))
            mapPreview.DrawMap();
    }
}
