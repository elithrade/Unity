using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public bool AutoUpdate;
    public DrawMode DrawMode;
    [Range(0, MeshGenerator.NumberOfSupportedChunkSizes - 1)]
    public int ChunkSizeIndex;
    [Range(0, MeshGenerator.NumberOfSupportedFlatshadedChunkSizes - 1)]
    public int FlatshadedChunkSizeIndex;
    [Range(0, MeshGenerator.NumberOfSupportedLOD - 1)]
    public int PreviewLOD;
    public TerrainData TerrainData;
    public NoiseData NoiseData;
    public TextureData TextureData;
    public Material TextureMaterial;

    private Queue<MapThreadInfo<MapData>> _pendingMapDataQueue;
    private Queue<MapThreadInfo<MeshData>> _pendingMeshDataQueue;
    private float[,] _falloffMap;

    private void Start()
    {
        _pendingMapDataQueue = new Queue<MapThreadInfo<MapData>>();
        _pendingMeshDataQueue = new Queue<MapThreadInfo<MeshData>>();
    }

    private void Update()
    {
        Process<MapData>(_pendingMapDataQueue);
        Process<MeshData>(_pendingMeshDataQueue);
    }

    private void Process<T>(Queue<MapThreadInfo<T>> queue)
        where T : class
    {
        // Locking inside Update method impacts performance a lot
        while (queue.Count > 0)
        {
            MapThreadInfo<T> info = queue.Dequeue();
            info.InvokeCallback();
        }
    }

    private MapData GenerateMapData(Vector2 centre)
    {
        // Ensure falloff map has the same map size
        int meshChunkSize = MeshChunkSize + 2;
        float[,] noiseMap = Noise.GenerateNoiseMap(meshChunkSize,
                                                   meshChunkSize,
                                                   NoiseData.Scale,
                                                   NoiseData.Seed,
                                                   NoiseData.Octave,
                                                   NoiseData.Persistence,
                                                   NoiseData.Lacunarity,
                                                   NoiseData.Offset + centre,
                                                   NoiseData.NormalizeMode);
        if (noiseMap == null)
            return null;

        if (TerrainData.UseFalloff)
        {
            if (_falloffMap == null)
                _falloffMap = FalloffGenerator.GenerateFalloffMap(meshChunkSize);

            for (int y = 0; y < meshChunkSize; y++)
            {
                for (int x = 0; x < meshChunkSize; x++)
                {
                    float currentHeight = noiseMap[x, y];
                    if (TerrainData.UseFalloff)
                    {
                        currentHeight = Mathf.Clamp01(currentHeight - _falloffMap[x, y]);
                        noiseMap[x, y] = currentHeight;
                    }
                }
            }
        }

        return new MapData(noiseMap);
    }

    public void RequestMapData(Action<MapData> onMapData, Vector2 centre)
    {
        Thread processMapDataThread = new Thread(() =>
        {
            lock (_pendingMapDataQueue)
            {
                MapData mapData = GenerateMapData(centre);
                _pendingMapDataQueue.Enqueue(new MapThreadInfo<MapData>(mapData, onMapData));
            }
        });

        processMapDataThread.Start();
    }

    public void RequestMeshData(Action<MeshData> onMeshData, int lod, MapData mapData)
    {
        Thread processMeshDataThread = new Thread(() =>
        {
            lock (_pendingMeshDataQueue)
            {
                MeshData meshData = MeshGenerator.Generate(
                    mapData.HeightMap,
                    TerrainData.HeightMultiplier,
                    TerrainData.HeightCurve,
                    lod,
                    TerrainData.UseFlatShading);

                _pendingMeshDataQueue.Enqueue(new MapThreadInfo<MeshData>(meshData, onMeshData));
            }
        });

        processMeshDataThread.Start();
    }

    public void DrawMap()
    {
        TextureData.SetMinMaxHeight(TextureMaterial, TerrainData.MinHeight, TerrainData.MaxHeight);

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (display == null)
            return;

        MapData mapData = GenerateMapData(Vector2.zero);
        if (DrawMode == DrawMode.Noise)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.HeightMap));
        }
        else if (DrawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.Generate(mapData.HeightMap, TerrainData.HeightMultiplier, TerrainData.HeightCurve, PreviewLOD, TerrainData.UseFlatShading));
        }
        else if (DrawMode == DrawMode.Falloff)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(FalloffGenerator.GenerateFalloffMap(MeshChunkSize)));
        }
    }

    private void Awake()
    {
        TextureData.Apply(TextureMaterial);
        TextureData.SetMinMaxHeight(TextureMaterial, TerrainData.MinHeight, TerrainData.MaxHeight);
    }

    private void OnValidate()
    {
        if (TerrainData != null)
        {
            TerrainData.OnValueUpdated -= OnValueUpdated;
            TerrainData.OnValueUpdated += OnValueUpdated;
        }
        if (NoiseData != null)
        {
            NoiseData.OnValueUpdated -= OnValueUpdated;
            NoiseData.OnValueUpdated += OnValueUpdated;
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

    public int MeshChunkSize
    {
        get
        {
            if (TerrainData.UseFlatShading)
                return MeshGenerator.SupportedFlatshadedChunkSizes[FlatshadedChunkSizeIndex] - 1;
            else
                return MeshGenerator.SupportedChunkSizes[ChunkSizeIndex] - 1;
        }
    }
}
