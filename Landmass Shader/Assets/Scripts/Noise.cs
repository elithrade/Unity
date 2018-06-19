using UnityEngine;

public static class Noise
{
    public enum NormalizeMode
    {
        Local,
        Global
    }

    public static float[,] GenerateNoiseMap(int width, int height, NoiseSettings settings, Vector2 sampleCentre)
    {
        // Seed allows us to generate different unique maps
        // The octave offsets are used to sample points from different locations
        System.Random random = new System.Random(settings.Seed);
        Vector2[] octaveOffsets = new Vector2[settings.Octave];

        float maximumPossibleHeight = 0;
        float maximumPerlinValue = 1;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < settings.Octave; i++)
        {
            // Offset allows us to scroll through the noise map
            float offsetX = random.Next(-100000, 100000) + settings.Offset.x + sampleCentre.x;
            float offsetY = random.Next(-100000, 100000) - settings.Offset.y - sampleCentre.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            // Note here we use the maximum perlin value which is 1
            maximumPossibleHeight += maximumPerlinValue * amplitude;
            amplitude *= settings.Persistence;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        // Calculate half size to generate points from the middle of the map
        float halfWidth = width / 2f;
        float halfHeight = height / 2f;

        float[,] noiseMap = new float[width, height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;
                // Octave refers to multiple layers of the noise maps add together
                // providing more detailed map but preserving the overall shape
                for (int i = 0; i < settings.Octave; i++)
                {
                    // Using same x, y will yield same perlin value
                    // The higher the frequency the further apart the sample points will be
                    // Fix landmass changing shape as adjusting offset by ensuring octave offset also affected by scale and frequency
                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / settings.Scale * frequency;
                    float sampleY = (y - halfHeight + octaveOffsets[i].y) / settings.Scale * frequency;
                    // Since perlin noise is between 0 and 1, we can generate negative values by * 2 - 1
                    float perlineValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                    // Increase the noise height by the perlin value of each octave
                    noiseHeight += perlineValue * amplitude;
                    // Persistence between 0 and 1 decreases the amplitude of each octave
                    amplitude *= settings.Persistence;
                    // Lacunarity is greater than 1 increases the frequency of each octave
                    frequency *= settings.Lacunarity;
                }

                if (noiseHeight > maxLocalNoiseHeight)
                    maxLocalNoiseHeight = noiseHeight;
                else if (noiseHeight < minLocalNoiseHeight)
                    minLocalNoiseHeight = noiseHeight;

                noiseMap[x, y] = noiseHeight;

                if (settings.NormalizeMode == NormalizeMode.Global)
                {
                    // Inverse of float perlineValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    float normalizedHeight = (noiseHeight + 1) / maximumPossibleHeight;
                    // Ensure height starts from 0
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }
            }
        }

        if (settings.NormalizeMode == NormalizeMode.Local)
        {
            // Since perlineValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 -1;
            // we need to normalise back to between 0 and 1
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float noiseHeight = noiseMap[x, y];
                    float inverseLerp = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseHeight);
                    noiseMap[x, y] = inverseLerp;
                }
            }
        }

        return noiseMap;
    }
}

[System.Serializable]
public class NoiseSettings
{
    public Noise.NormalizeMode NormalizeMode;
    public float Scale = 50;
    public int Octave = 6;
    [Range(0, 1)]
    public float Persistence = 0.6f;
    public float Lacunarity = 2;
    public int Seed;
    public Vector2 Offset;

    public void Validate()
    {
        Scale = Mathf.Max(Scale, 0.01f);
        Octave = Mathf.Max(Octave, 1);
        Lacunarity = Mathf.Max(Lacunarity, 1);
        // Persistence must between 0 and 1
        Persistence = Mathf.Clamp01(Persistence);
    }
}
