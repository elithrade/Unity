using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int Width;
    public int Height;
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

    public void GenerateMap()
    {
        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (display == null)
            return;

        float[,] noiseMap = Noise.GenerateNoiseMap(Width, Height, Scale, Seed,
                                                   Octave, Persistence, Lacunarity, Offset);
        if (noiseMap == null)
            return;

        Color[] colorMap = new Color[Width * Height];
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < Regions.Length; i++)
                {
                    Region region = Regions[i];
                    if (currentHeight <= region.Height)
                    {
                        colorMap[y * Width + x] = region.Color;
                        break;
                    }
                }
            }
        }

        if (DrawMode == DrawMode.Noise)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        }
        else if (DrawMode == DrawMode.Color || DrawMode == DrawMode.Mesh)
        {
            Texture2D texture = TextureGenerator.TextureFromColorMap(colorMap, Width, Height);
            if (DrawMode == DrawMode.Color)
                display.DrawTexture(texture);
            else
                display.DrawMesh(MeshGenerator.Generate(noiseMap), texture);
        }
    }

    public void OnValidate()
    {
        if (Width < 1)
            Width = 1;
        if (Height < 1)
            Height = 1;
        if (Octave < 1)
            Octave = 1;
        if (Lacunarity < 1)
            Lacunarity = 1;
        if (Scale < 0.001f)
            Scale = 0.001f;
    }
}
