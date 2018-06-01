using UnityEngine;

public static class Noise
{
    public enum NormalizeMode
    {
        Local,
        Global
    }

    public static float[,] GenerateNoiseMap(int width, int height, float scale, int seed,
                                            int octave, float persistence, float lacunarity, Vector2 offset, NormalizeMode normalizeMode)
    {
        // Seed allows us to generate different unique maps
        // The octave offsets are used to sample points from different locations
        System.Random random = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octave];

        float maximumPossibleHeight = 0;
        float maximumPerlinValue = 1;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < octave; i++)
        {
            // Offset allows us to scroll through the noise map
            float offsetX = random.Next(-100000, 100000) + offset.x;
            float offsetY = random.Next(-100000, 100000) - offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            // Note here we use the maximum perlin value which is 1
            maximumPossibleHeight += maximumPerlinValue * amplitude;
            amplitude *= persistence;
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
                for (int i = 0; i < octave; i++)
                {
                    // Using same x, y will yield same perlin value
                    // The higher the frequency the further apart the sample points will be
                    // Fix landmass changing shape as adjusting offset by ensuring octave offset also affected by scale and frequency
                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / scale * frequency;
                    float sampleY = (y - halfHeight + octaveOffsets[i].y) / scale * frequency;
                    // Since perlin noise is between 0 and 1, we can generate negative values by * 2 - 1
                    float perlineValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                    // Increase the noise height by the perlin value of each octave
                    noiseHeight += perlineValue * amplitude;
                    // Persistence between 0 and 1 decreases the amplitude of each octave
                    amplitude *= persistence;
                    // Lacunarity is greater than 1 increases the frequency of each octave
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxLocalNoiseHeight)
                    maxLocalNoiseHeight = noiseHeight;
                else if (noiseHeight < minLocalNoiseHeight)
                    minLocalNoiseHeight = noiseHeight;

                noiseMap[x, y] = noiseHeight;
            }
        }

        // Since perlineValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 -1;
        // we need to normalise back to between 0 and 1
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float noiseHeight = noiseMap[x, y];
                if (normalizeMode == NormalizeMode.Local)
                {
                    float inverseLerp = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseHeight);
                    noiseMap[x, y] = inverseLerp;
                }
                else if (normalizeMode == NormalizeMode.Global)
                {
                    // Inverse of float perlineValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    float normalizedHeight = (noiseHeight + 1) / maximumPossibleHeight;
                    // Ensure height starts from 0
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }
            }
        }

        return noiseMap;
    }
}
