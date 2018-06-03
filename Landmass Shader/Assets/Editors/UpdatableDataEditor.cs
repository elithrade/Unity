using UnityEngine;
using UnityEditor;

// Specify true to indicate we want it to work with derived types
[CustomEditor(typeof(UpdatableData), true)]
public class UpdatableDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        UpdatableData data = (UpdatableData) target;
        if (GUILayout.Button("Update"))
        {
            data.NotifyValueChanged();
            EditorUtility.SetDirty(target);
        }
    }
}