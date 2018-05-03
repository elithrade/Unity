using UnityEngine;

public static class MeshGenerator
{
    public static MeshData Generate(float[,] heightMap)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;

        MeshData meshData = new MeshData(width, height);

        int vertexIndex = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 vertex = new Vector3(topLeftX + x, heightMap[x,y], topLeftZ - y);
                meshData.Vertices[vertexIndex] = vertex;

                Vector2 uv = new Vector2(1 - x / (float) width, y / (float) height);
                meshData.Uvs[vertexIndex] = uv;

                // The last row and column cannot form any triangle
                if (x < width - 1 && y < height - 1)
                {
                    // Adding triangles clockwise
                    // i, i + width + 1, i + width
                    // i + width + 1, i, i + 1
                    meshData.AddTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
                    meshData.AddTriangle(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }

        return meshData;
    }
}
