using UnityEngine;

public static class TextureGenerator
{
    public static Texture2D TextureFromColorMap(Color[] colourMap, int width, int height)
    {
        // The width and height are the dimension of 2D array
        Texture2D texture = new Texture2D(width, height);
        // Setting pixels using array is much more efficient than setting single pixel
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Point;
        texture.SetPixels(colourMap);
        texture.Apply();

        return texture;
    }

    public static Texture2D TextureFromHeightMap(HeightMap heightMap)
    {
        int width = heightMap.Values.GetLength(0);
        int height = heightMap.Values.GetLength(1);

        Color[] colourMap = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Converts 2D array to 1D then interpolate between black and white using the noise value
                colourMap[y * width + x] = Color.Lerp(
                    Color.black,
                    Color.white,
                    // heightMap.Values are scales by height curve etc. re-sampling back to between 0 and 1
                    Mathf.InverseLerp(heightMap.MinValue, heightMap.MaxValue, heightMap.Values[x, y]));
            }
        }

        return TextureFromColorMap(colourMap, width, height);
    }
}