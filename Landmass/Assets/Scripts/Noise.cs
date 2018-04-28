using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoiseMap(int width, int height, float scale)
    {
        if (width < 0 || height < 0)
            return null;

        if (scale <= 0)
            scale = 0.001f;

        float[,] noiseMap = new float[width, height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Using same x, y will yield same perlin value
                float sampleX = x / scale;
                float sampleY = y / scale;
                float noise = Mathf.PerlinNoise(sampleX, sampleY);

                noiseMap[x,y] = noise;
            }
        }

        return noiseMap;
    }
}
