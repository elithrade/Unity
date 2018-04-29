using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int Width;
    public int Height;
    public float Scale;
    public int Octave;
    public float Persistence;
    public float Lacunarity;
    public int Seed;
    public Vector2 Offset;
    public bool AutoUpdate;

    public void GenerateMap()
    {
        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (display == null)
            return;

        float[,] noiseMap = Noise.GenerateNoiseMap(Width, Height, Scale, Seed,
                                                   Octave, Persistence, Lacunarity, Offset);
        if (noiseMap == null)
            return;

        display.Draw(noiseMap);
    }
}
