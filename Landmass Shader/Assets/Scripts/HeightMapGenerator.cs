using UnityEngine;

public static class HeightMapGenerator
{
    public static HeightMap GenerateHeightMap(int width, int height, HeightMapSettings settings, Vector2 sampleCentre)
    {
        float[,] values = Noise.GenerateNoiseMap(width, height, settings.NoiseSettings, sampleCentre);
        // Make local copy to make sure it is thread safe
        AnimationCurve localHeightCurve = new AnimationCurve(settings.HeightCurve.keys);

        float minValue = float.MaxValue;
        float maxValue = float.MinValue;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                values[i, j] *= localHeightCurve.Evaluate(values[i, j]) * settings.HeightMultiplier;

                float value = values[i, j];
                if (value < minValue)
                    minValue = value;
                if (value > maxValue)
                    maxValue = value;
            }
        }

        return new HeightMap(values, minValue, maxValue);
    }
}

public struct HeightMap
{
    public readonly float[,] Values;
    public readonly float MinValue;
    public readonly float MaxValue;

    public HeightMap(float[,] values, float minValue, float maxValue)
    {
        Values = values;
        MinValue = minValue;
        MaxValue = maxValue;
    }
}