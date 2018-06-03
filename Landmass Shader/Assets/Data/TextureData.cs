using System;
using UnityEngine;

[CreateAssetMenu()]
public class TextureData : UpdatableData
{
    public Color[] BaseColours;
    [Range(0,1)]
    // Each of the float determines the starting color in BaseColors array
    public float[] BaseStartHeights;

    float _savedMinHeight;
    float _savedMaxHeight;

    internal void Apply(Material material)
    {
        material.SetInt("baseColourCount", BaseColours.Length);
        material.SetColorArray("baseColours", BaseColours);
        material.SetFloatArray("baseStartHeights", BaseStartHeights);

        SetMinMaxHeight(material, _savedMinHeight, _savedMaxHeight);
    }

    public void SetMinMaxHeight(Material material, float minHeight, float maxHeight)
    {
        _savedMinHeight = minHeight;
        _savedMaxHeight = maxHeight;

        // SetFloat property name has to match what's defined in the shader
        material.SetFloat("minHeight", minHeight);
        material.SetFloat("maxHeight", maxHeight);
    }
}
