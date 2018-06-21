using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public bool AutoUpdate;
    public DrawMode DrawMode;
    [Range(0, MeshSettings.NumberOfSupportedLOD - 1)]
    public int PreviewLOD;
    public MeshSettings MeshSettings;
    public HeightMapSettings HeightMapSettings;
    public TextureData TextureData;
    public Material TextureMaterial;
    private float[,] _falloffMap;

    private void Start()
    {
    }

    public void DrawMap()
    {
        TextureData.SetMinMaxHeight(TextureMaterial, HeightMapSettings.MinHeight, HeightMapSettings.MaxHeight);

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (display == null)
            return;

        HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(MeshSettings.NumberOfVerticesPerLine, MeshSettings.NumberOfVerticesPerLine, HeightMapSettings, Vector2.zero);
        if (DrawMode == DrawMode.Noise)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(heightMap.Values));
        }
        else if (DrawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.Generate(heightMap.Values, MeshSettings, PreviewLOD));
        }
        else if (DrawMode == DrawMode.Falloff)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(FalloffGenerator.GenerateFalloffMap(MeshSettings.NumberOfVerticesPerLine)));
        }
    }

    private void OnValidate()
    {
        if (MeshSettings != null)
        {
            MeshSettings.OnValueUpdated -= OnValueUpdated;
            MeshSettings.OnValueUpdated += OnValueUpdated;
        }
        if (HeightMapSettings != null)
        {
            HeightMapSettings.OnValueUpdated -= OnValueUpdated;
            HeightMapSettings.OnValueUpdated += OnValueUpdated;
        }
        if (TextureData != null)
        {
            TextureData.OnValueUpdated -= OnTextureUpdated;
            TextureData.OnValueUpdated += OnTextureUpdated;
        }
    }

    private void OnTextureUpdated()
    {
        TextureData.Apply(TextureMaterial);
    }

    private void OnValueUpdated()
    {
        if (Application.isPlaying)
            return;

        DrawMap();
    }
}
