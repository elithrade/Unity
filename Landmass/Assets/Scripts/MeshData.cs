using UnityEngine;

public class MeshData
{
    public readonly Vector3[] Vertices;
    public Vector2[] Uvs;
    private readonly int[] _triangles;
    private int _triangleIndex;
    private int _vertexIndex;

    public MeshData(int width, int height)
    {
        Vertices = new Vector3[width * height];
        Uvs = new Vector2[width * height];

        // Number of squares equals (width - 1) * (height - 1)
        // Each square contains 2 triangles with 3 vertices each
        int triangleArraySize = (width - 1) * (height - 1) * 6;
        _triangles = new int[triangleArraySize];
        // Uv map, one for each vertex
        // Foreach vertex where it is in relation to the rest of the map as a percentage for both the x and y axis
    }

    public void AddTriangle(int a, int b, int c)
    {
        _triangles[_triangleIndex] = a;
        _triangles[_triangleIndex + 1] = b;
        _triangles[_triangleIndex + 2] = c;
        _triangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = Vertices;
        mesh.triangles = _triangles;
        mesh.uv = Uvs;
        mesh.normals = CalculateNormals();

        return mesh;
    }

    private Vector3[] CalculateNormals()
    {
        Vector3[] normals = new Vector3[Vertices.Length];
        // Calculate normal for each triangle
        int numberOfTriangles = _triangles.Length / 3;
        for (int i = 0; i < numberOfTriangles; i++)
        {
            int normalTriangleIndex = i * 3;
            int vertexIndexA = _triangles[normalTriangleIndex];
            int vertexIndexB = _triangles[normalTriangleIndex + 1];
            int vertexIndexC = _triangles[normalTriangleIndex + 2];

            Vector3 triangleNormal = CalculateSurfaceNormalFromTriangles(vertexIndexA, vertexIndexB, vertexIndexC);

            // In unity we need to provide normal on per vertex basis
            // Each vertex normal is the average of the normals of triangles to which the vertex belongs
            //
            // We want to add the triangle normal to each of the vertices that are part of the triangle
            normals[vertexIndexA] += triangleNormal;
            normals[vertexIndexA].Normalize();

            normals[vertexIndexB] += triangleNormal;
            normals[vertexIndexB].Normalize();

            normals[vertexIndexC] += triangleNormal;
            normals[vertexIndexC].Normalize();
        }

        return normals;
    }

    private Vector3 CalculateSurfaceNormalFromTriangles(int indexA, int indexB, int indexC)
    {
        Vector3 pointA =  Vertices[indexA];
        Vector3 pointB =  Vertices[indexB];
        Vector3 pointC =  Vertices[indexC];

        Vector3 sideAB = pointB - pointA;
        Vector3 sideAC = pointC - pointA;

        return Vector3.Cross(sideAB, sideAC).normalized;
    }
}