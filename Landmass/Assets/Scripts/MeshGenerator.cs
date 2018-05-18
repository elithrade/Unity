using UnityEngine;

public static class MeshGenerator
{
    // Note that this method is called from a thread pool thread
    public static MeshData Generate(
        float[,] heightMap,
        float heightMultiplier,
        AnimationCurve heightCurve,
        int levelOfDetail)
    {
        int increment = levelOfDetail == 0 ? 1 : levelOfDetail * 2;

        int borderedSize = heightMap.GetLength(0);
        int meshSize = borderedSize - 2 * increment;
        int originalMeshSize = borderedSize - 2;

        float topLeftX = (originalMeshSize - 1) / -2f;
        float topLeftZ = (originalMeshSize - 1) / 2f;

        MeshData meshData = new MeshData(borderedSize);

        // To calculate the normals correctly between chunks
        // we need to take into account the bordered vertices
        // labelled by the negative indices which are
        // excluded from the final mesh.
        // We use the variable borderedSize and meshSize, in
        // this case borderedSize = 5, meshSize = 3.
        // In general meshSize = borderedSize - 2
        // -1  -2  -3  -4   -5
        // -6   0   1   2   -7
        // -8   3   4   5   -9
        // -10  6   7   8  -11
        // -12 -13 -14 -15 -16
        int[,] vertexIndicesMap = new int[borderedSize, borderedSize];

        int meshVertexIndex = 0;
        int borderVertexIndex = -1;

        for (int y = 0; y < borderedSize; y = y + increment)
        {
            for (int x = 0; x < borderedSize; x = x + increment)
            {
                bool isBorderIndex = y == 0 || y == borderedSize - 1 || x == 0 || x == borderedSize - 1;
                if (isBorderIndex)
                {
                    vertexIndicesMap[x, y] = borderVertexIndex;
                    borderVertexIndex--;
                }
                else
                {
                    vertexIndicesMap[x, y] = meshVertexIndex;
                    meshVertexIndex++;
                }
            }
        }

        for (int y = 0; y < borderedSize; y = y + increment)
        {
            for (int x = 0; x < borderedSize; x = x + increment)
            {
                int vertexIndex = vertexIndicesMap[x, y];

                // Ensure uvs are properly centred by subtracting the increment
                Vector2 uv = new Vector2((x - increment) / (float)meshSize, (y - increment) / (float)meshSize);
                float height = heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier;

                Vector3 vertex = new Vector3(topLeftX + uv.x * originalMeshSize, height, topLeftZ - uv.y * originalMeshSize);
                meshData.AddVertex(vertex, uv, vertexIndex);

                // The last row and column cannot form any triangle
                if (x < borderedSize - 1 && y < borderedSize - 1)
                {
                    // a(x,y)------b(x+i,y)
                    // |           |
                    // |           |
                    // |           |
                    // c(x,y+i)----d(x+i,y+i)
                    // Triangles are clockwise adc, dab
                    int a = vertexIndicesMap[x, y];
                    int b = vertexIndicesMap[x + increment, y];
                    int c = vertexIndicesMap[x, y + increment];
                    int d = vertexIndicesMap[x + increment, y + increment];
                    meshData.AddTriangle(a, d, c);
                    meshData.AddTriangle(d, a, b);
                }

                vertexIndex++;
            }
        }

        meshData.BakeNormals();

        return meshData;
    }
}
