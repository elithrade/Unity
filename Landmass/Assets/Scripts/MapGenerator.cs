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

    private int _meshChunkSize = 255;

    private MapData GenerateMapData()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(_meshChunkSize, _meshChunkSize, Scale, Seed,
                                                   Octave, Persistence, Lacunarity, Offset);
        if (noiseMap == null)
            return null;;

        Color[] colorMap = new Color[_meshChunkSize * _meshChunkSize];
        for (int y = 0; y < _meshChunkSize; y++)
        {
            for (int x = 0; x < _meshChunkSize; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < Regions.Length; i++)
                {
                    Region region = Regions[i];
                    if (currentHeight <= region.Height)
                    {
                        colorMap[y * _meshChunkSize + x] = region.Color;
                        break;
                    }
                }
            }
        }

        return new MapData(noiseMap, colorMap);
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
            Texture2D texture = TextureGenerator.TextureFromColorMap(mapData.ColorMap, _meshChunkSize, _meshChunkSize);
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
