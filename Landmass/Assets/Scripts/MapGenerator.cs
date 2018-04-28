using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int Width;
    public int Height;
    public float Scale;
    public bool AutoUpdate;

    public void GenerateMap()
    {
        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (display == null)
            return;

        float[,] noiseMap = Noise.GenerateNoiseMap(Width, Height, Scale);
        if (noiseMap == null)
            return;

        display.Draw(noiseMap);
    }
}
