using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Target is the object we are inspecting
        MapGenerator mapGenerator = (MapGenerator) target;
        if (DrawDefaultInspector())
        {
            if (mapGenerator.AutoUpdate)
                mapGenerator.GenerateMap();
        }

        if (GUILayout.Button("Generate"))
            mapGenerator.GenerateMap();
    }
}
