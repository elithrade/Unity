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
    public Region[] Regions;
    public float HeightMultiplier;
    public AnimationCurve HeightCurve;
    [Range(0, 6)]
    public int LevelOfDetail;
    public static int MeshChunkSize = 241;

    private Queue<MapThreadInfo<MapData>> _pendingMapDataQueue;

    private void Start()
    {
        _pendingMapDataQueue = new Queue<MapThreadInfo<MapData>>();
    }

    private void Update()
    {
        lock (_pendingMapDataQueue)
        {
            while (_pendingMapDataQueue.Count > 0)
            {
                MapThreadInfo<MapData> info = _pendingMapDataQueue.Dequeue();
                info.InvokeCallback();
            }
        }
    }

    private MapData GenerateMapData()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(MeshChunkSize, MeshChunkSize, Scale, Seed,
                                                   Octave, Persistence, Lacunarity, Offset);
        if (noiseMap == null)
            return null; ;

        Color[] colorMap = new Color[MeshChunkSize * MeshChunkSize];
        for (int y = 0; y < MeshChunkSize; y++)
        {
            for (int x = 0; x < MeshChunkSize; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < Regions.Length; i++)
                {
                    Region region = Regions[i];
                    if (currentHeight <= region.Height)
                    {
                        colorMap[y * MeshChunkSize + x] = region.Color;
                        break;
                    }
                }
            }
        }

        return new MapData(noiseMap, colorMap);
    }

    public void RequestMapData(Action<MapData> onMapData)
    {
        Thread processMapDataThread = new Thread(_ => Enqueue(onMapData));
        processMapDataThread.Start();
    }

    private void Enqueue(Action<MapData> onMapData)
    {
        lock (_pendingMapDataQueue)
        {
            MapData mapData = GenerateMapData();
            _pendingMapDataQueue.Enqueue(new MapThreadInfo<MapData>(mapData, onMapData));
        }
    }

    public void DrawMap()
    {
        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (display == null)
            return;

        MapData mapData = GenerateMapData();
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
                display.DrawMesh(MeshGenerator.Generate(mapData.HeightMap, HeightMultiplier, HeightCurve, LevelOfDetail), texture);
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
    }
}
