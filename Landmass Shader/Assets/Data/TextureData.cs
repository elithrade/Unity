using System;
using UnityEngine;

[CreateAssetMenu()]
public class TextureData : UpdatableData
{
    internal void Apply(Material textureMaterial)
    {
    }

    public void SetMinMaxHeight(Material material, float minHeight, float maxHeight)
    {
        // SetFloat property name has to match what's defined in the shader
        material.SetFloat("minHeight", minHeight);
        material.SetFloat("maxHeight", maxHeight);
    }
}
