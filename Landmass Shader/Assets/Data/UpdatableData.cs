using System;
using UnityEngine;

public class UpdatableData : ScriptableObject
{
    public event Action OnValueUpdated;
    public bool AutoUpdate;

    protected virtual void OnValidate()
    {
        if (AutoUpdate)
            NotifyValueChanged();
    }

    public void NotifyValueChanged()
    {
        if (OnValueUpdated != null)
            OnValueUpdated();
    }
}
