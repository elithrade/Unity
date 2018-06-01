using UnityEngine;

[CreateAssetMenu()]
public class TerrainData : UpdatableData
{
    public bool UseFalloff;
    public bool UseFlatShading;
    public float HeightMultiplier;
    // Scales on the y axis
    public AnimationCurve HeightCurve;
    // Scales on the x, y, and z axis
    public float UniformScale = 2.5f;

    public float MinHeight
    {
        get
        {
            return UniformScale * HeightMultiplier * HeightCurve.Evaluate(0);
        }
    }

    public float MaxHeight
    {
        get
        {
            return UniformScale * HeightMultiplier * HeightCurve.Evaluate(1);
        }
    }
}
