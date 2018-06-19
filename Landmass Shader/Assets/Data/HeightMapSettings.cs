using UnityEngine;

[CreateAssetMenu()]
public class HeightMapSettings : UpdatableData
{
    public NoiseSettings NoiseSettings;
    public float HeightMultiplier;
    public bool UseFalloff;
    // Scales on the y axis
    public AnimationCurve HeightCurve;

    public float MinHeight
    {
        get
        {
            return HeightMultiplier * HeightCurve.Evaluate(0);
        }
    }

    public float MaxHeight
    {
        get
        {
            return HeightMultiplier * HeightCurve.Evaluate(1);
        }
    }

    #if UNITY_EDITOR

    protected override void OnValidate()
    {
        NoiseSettings.Validate();
        base.OnValidate();
    }

    #endif
}
