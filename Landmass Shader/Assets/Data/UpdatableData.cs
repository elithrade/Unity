using System;
using UnityEngine;

public class UpdatableData : ScriptableObject
{
    public event Action OnValueUpdated;
    public bool AutoUpdate;

    #if UNITY_EDITOR

    protected virtual void OnValidate()
    {
        if (AutoUpdate)
            UnityEditor.EditorApplication.update += NotifyValueChanged;
    }

    public void NotifyValueChanged()
    {
        if (OnValueUpdated != null)
        {
            // Workaround for delaying shader updated after compile
            UnityEditor.EditorApplication.update -= NotifyValueChanged;
            OnValueUpdated();
        }
    }

    #endif
}
