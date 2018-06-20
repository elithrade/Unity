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

    private Queue<MapThreadInfo<HeightMap>> _pendingMapDataQueue;
    private Queue<MapThreadInfo<MeshData>> _pendingMeshDataQueue;
    private float[,] _falloffMap;

    private void Start()
    {
        TextureData.Apply(TextureMaterial);
        TextureData.SetMinMaxHeight(TextureMaterial, HeightMapSettings.MinHeight, HeightMapSettings.MaxHeight);

        _pendingMapDataQueue = new Queue<MapThreadInfo<HeightMap>>();
        _pendingMeshDataQueue = new Queue<MapThreadInfo<MeshData>>();
    }

    private void Update()
    {
        Process<HeightMap>(_pendingMapDataQueue);
        Process<MeshData>(_pendingMeshDataQueue);
    }

    private void Process<T>(Queue<MapThreadInfo<T>> queue)
    {
        // Locking inside Update method impacts performance a lot
        while (queue.Count > 0)
        {
            MapThreadInfo<T> info = queue.Dequeue();
            info.InvokeCallback();
        }
    }

    public void RequestMapData(Action<HeightMap> onMapData, Vector2 centre)
    {
        Thread processMapDataThread = new Thread(() =>
        {
            HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(MeshSettings.NumberOfVerticesPerLine, MeshSettings.NumberOfVerticesPerLine, HeightMapSettings, centre);
            // lock (_pendingMapDataQueue)
            {
                _pendingMapDataQueue.Enqueue(new MapThreadInfo<HeightMap>(heightMap, onMapData));
            }
        });

        processMapDataThread.Start();
    }

    public void RequestMeshData(Action<MeshData> onMeshData, int lod, HeightMap mapData)
    {
        Thread processMeshDataThread = new Thread(() =>
        {
            MeshData meshData = MeshGenerator.Generate(
                mapData.Values,
                MeshSettings,
                lod);

            // lock (_pendingMeshDataQueue)
            {
                _pendingMeshDataQueue.Enqueue(new MapThreadInfo<MeshData>(meshData, onMeshData));
            }
        });

        processMeshDataThread.Start();
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
