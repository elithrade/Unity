using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public float Scale;
    public int Octave;
    [Range(0, 1)]
    public float Persistence;
    public float Lacunarity;
    public int Seed;
    public Vector2 Offset;
    public bool AutoUpdate;
    public DrawMode DrawMode;
    public Noise.NormalizeMode NormalizeMode;
    public Region[] Regions;
    public float HeightMultiplier;
    public AnimationCurve HeightCurve;
    [Range(0, 6)]
    public int PreviewLOD;
    public bool UseFalloff;
    public bool UseFlatShading;

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
        float[,] noiseMap = Noise.GenerateNoiseMap(MeshChunkSize + 2, MeshChunkSize + 2, Scale, Seed,
                                                   Octave, Persistence, Lacunarity, centre + Offset, NormalizeMode);
        if (noiseMap == null)
            return null; ;

        Color[] colorMap = new Color[MeshChunkSize * MeshChunkSize];
        for (int y = 0; y < MeshChunkSize; y++)
        {
            for (int x = 0; x < MeshChunkSize; x++)
            {
                float currentHeight = noiseMap[x, y];
                if (UseFalloff)
                {
                    currentHeight = Mathf.Clamp01(currentHeight - _falloffMap[x, y]);
                    noiseMap[x, y] = currentHeight;
                }

                for (int i = 0; i < Regions.Length; i++)
                {
                    Region region = Regions[i];
                    if (currentHeight >= region.Height)
                        colorMap[y * MeshChunkSize + x] = region.Color;
                    else
                        break;
                }
            }
        }

        return new MapData(noiseMap, colorMap);
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
                    HeightMultiplier,
                    HeightCurve,
                    lod,
                    UseFlatShading);

                _pendingMeshDataQueue.Enqueue(new MapThreadInfo<MeshData>(meshData, onMeshData));
            }
        });

        processMeshDataThread.Start();
    }

    public void DrawMap()
    {
        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (display == null)
            return;

        MapData mapData = GenerateMapData(Vector2.zero);
        if (DrawMode == DrawMode.Noise)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.HeightMap));
        }
        else if (DrawMode == DrawMode.Color || DrawMode == DrawMode.Mesh)
        {
            Texture2D texture = TextureGenerator.TextureFromColorMap(mapData.ColorMap, MeshChunkSize, MeshChunkSize);
            if (DrawMode == DrawMode.Color)
                display.DrawTexture(texture);
            else
                display.DrawMesh(MeshGenerator.Generate(mapData.HeightMap, HeightMultiplier, HeightCurve, PreviewLOD, UseFlatShading), texture);
        }
        else if (DrawMode == DrawMode.Falloff)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(FalloffGenerator.GenerateFalloffMap(MeshChunkSize)));
        }
    }

    public void OnValidate()
    {
        if (Octave < 1)
            Octave = 1;
        if (Lacunarity < 1)
            Lacunarity = 1;
        if (Scale < 0.001f)
            Scale = 0.001f;

        _falloffMap = FalloffGenerator.GenerateFalloffMap(MeshChunkSize);
    }

    private void Awake()
    {
        _falloffMap = FalloffGenerator.GenerateFalloffMap(MeshChunkSize);
    }

    public int MeshChunkSize
    {
        get
        {
            if (UseFlatShading)
                return 95;
            else
                return 239;
        }
    }
}
