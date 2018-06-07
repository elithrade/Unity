using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu()]
public class TextureData : UpdatableData
{
    public Layer[] Layers;
    private float _savedMinHeight;
    private float _savedMaxHeight;

    const int _textureSize = 512;
    const TextureFormat _textureFormat = TextureFormat.RGB565;

    internal void Apply(Material material)
    {
        material.SetInt("layerCount", Layers.Length);
        material.SetColorArray("baseColours", Layers.Select(x => x.Tint).ToArray());
        material.SetFloatArray("baseStartHeights", Layers.Select(x => x.StartHeight).ToArray());
        material.SetFloatArray("baseBlends", Layers.Select(x => x.BlendStrength).ToArray());
        material.SetFloatArray("baseColourStrength", Layers.Select(x => x.TintStrength).ToArray());
        material.SetFloatArray("baseTextureScales", Layers.Select(x => x.TextureScale).ToArray());
        Texture2DArray texturesArray = GenerateTextureArray(Layers.Select(x => x.Texture).ToArray());
        material.SetTexture("baseTextures", texturesArray);

        SetMinMaxHeight(material, _savedMinHeight, _savedMaxHeight);
    }

    private Texture2DArray GenerateTextureArray(Texture2D[] textures)
    {
        Texture2DArray textureArray = new Texture2DArray(_textureSize, _textureSize, textures.Length, _textureFormat, true);
        for (int i = 0; i < textures.Length; i++)
            textureArray.SetPixels(textures[i].GetPixels(), i);

        textureArray.Apply();

        return textureArray;
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
        public Texture2D Texture;
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
