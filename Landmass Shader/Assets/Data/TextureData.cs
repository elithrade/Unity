using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu()]
public class TextureData : UpdatableData
{
    public Layer[] Layers;
    float _savedMinHeight;
    float _savedMaxHeight;

    internal void Apply(Material material)
    {
        material.SetInt("layerCount", Layers.Length);
        material.SetColorArray("baseColours", Layers.Select(x => x.Tint).ToArray());
        material.SetFloatArray("baseStartHeights", Layers.Select(x => x.StartHeight).ToArray());
        material.SetFloatArray("baseBlends", Layers.Select(x => x.BlendStrength).ToArray());
        material.SetFloatArray("baseColourStrength", Layers.Select(x => x.TintStrength).ToArray());
        material.SetFloatArray("baseTextureScale", Layers.Select(x => x.TextureScale).ToArray());

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

    [Serializable]
    public class Layer
    {
        public Texture Texture;
        public Color Tint;
        [Range(0, 1)]
        public float TintStrength;
        [Range(0, 1)]
        public float StartHeight;
        [Range(0, 1)]
        public float BlendStrength;
        public float TextureScale;
    }
}
