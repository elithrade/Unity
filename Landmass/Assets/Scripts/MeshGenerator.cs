using System;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData Generate(
        float[,] heightMap,
        float heightMultiplier,
        AnimationCurve heightCurve,
        int levelOfDetail)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        int increment = levelOfDetail == 0 ? 1 : levelOfDetail * 2;
        int verticesPerLine = (width - 1) / increment + 1;

        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;

        MeshData meshData = new MeshData(width, height);

        int vertexIndex = 0;
        for (int y = 0; y < height; y = y + increment)
        {
            for (int x = 0; x < width; x = x + increment)
            {
                float heightOnCurve = heightCurve.Evaluate(heightMap[x, y]);
                Vector3 vertex = new Vector3(topLeftX + x, heightOnCurve * heightMultiplier, topLeftZ - y);
                meshData.Vertices[vertexIndex] = vertex;

                Vector2 uv = new Vector2(x / (float)width, y / (float)height);
                meshData.Uvs[vertexIndex] = uv;

                // The last row and column cannot form any triangle
                if (x < width - 1 && y < height - 1)
                {
                    // Adding triangles clockwise
                    // i, i + width + 1, i + width
                    // i + width + 1, i, i + 1
                    meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
                    meshData.AddTriangle(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }

        return meshData;
    }
}
