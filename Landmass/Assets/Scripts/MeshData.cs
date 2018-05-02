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
        mesh.RecalculateNormals();

        return mesh;
    }
}