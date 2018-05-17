using System;
using UnityEngine;

public class MeshData
{
    private readonly Vector3[] _vertices;
    private readonly Vector3[] _borderVertices;
    private Vector2[] _uvs;
    private readonly int[] _triangles;
    private readonly int[] _borderTriangles;
    private int _triangleIndex;
    private int _borderTriangleIndex;
    private int _vertexIndex;
    private Vector3[] _bakedNormals;

    public MeshData(int verticesPerLine)
    {
        _vertices = new Vector3[verticesPerLine * verticesPerLine];
        _borderVertices = new Vector3[verticesPerLine * 4 + 4];

        // Uv map, one for each vertex
        // Foreach vertex where it is in relation to the rest of the map as a percentage for both the x and y axis
        _uvs = new Vector2[verticesPerLine * verticesPerLine];

        // Number of squares equals (width - 1) * (height - 1)
        // Each square contains 2 triangles with 3 vertices each
        int triangleArraySize = (verticesPerLine - 1) * (verticesPerLine - 1) * 6;
        _triangles = new int[triangleArraySize];
        _borderTriangles = new int[6 * 4 * verticesPerLine];
    }

    public void AddVertex(Vector3 vertex, Vector2 uv, int vertexIndex)
    {
        if (vertexIndex < 0)
        {
            // Border index, invert index and - 1 to start with 0
            _borderVertices[-vertexIndex - 1] = vertex;
        }
        else
        {
            _vertices[vertexIndex] = vertex;
            _uvs[vertexIndex] = uv;
        }
    }

    public void AddTriangle(int a, int b, int c)
    {
        if (a < 0 || b < 0 || c < 0)
        {
            // Border triangle
            _borderTriangles[_borderTriangleIndex] = a;
            _borderTriangles[_borderTriangleIndex + 1] = b;
            _borderTriangles[_borderTriangleIndex + 2] = c;
            _borderTriangleIndex += 3;
        }
        else
        {
            _triangles[_triangleIndex] = a;
            _triangles[_triangleIndex + 1] = b;
            _triangles[_triangleIndex + 2] = c;
            _triangleIndex += 3;
        }
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = _vertices;
        mesh.triangles = _triangles;
        mesh.uv = _uvs;
        mesh.normals = _bakedNormals;

        return mesh;
    }

    private Vector3[] CalculateNormals()
    {
        Vector3[] normals = new Vector3[_vertices.Length];
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
            normals[vertexIndexB] += triangleNormal;
            normals[vertexIndexC] += triangleNormal;
        }

        int numberOfBorderTriangles = _borderTriangles.Length / 3;
        for (int i = 0; i < numberOfBorderTriangles; i++)
        {
            int normalTriangleIndex = i * 3;
            int vertexIndexA = _borderTriangles[normalTriangleIndex];
            int vertexIndexB = _borderTriangles[normalTriangleIndex + 1];
            int vertexIndexC = _borderTriangles[normalTriangleIndex + 2];

            Vector3 triangleNormal = CalculateSurfaceNormalFromTriangles(vertexIndexA, vertexIndexB, vertexIndexC);

            if (vertexIndexA >= 0)
                normals[vertexIndexA] += triangleNormal;

            if (vertexIndexB >= 0)
                normals[vertexIndexB] += triangleNormal;

            if (vertexIndexC >= 0)
                normals[vertexIndexC] += triangleNormal;
        }

        for (int i = 0; i < normals.Length; i++)
            normals[i].Normalize();

        return normals;
    }

    internal void BakeNormals()
    {
        _bakedNormals = CalculateNormals();
    }

    private Vector3 CalculateSurfaceNormalFromTriangles(int indexA, int indexB, int indexC)
    {
        Vector3 pointA = indexA < 0 ? _borderVertices[-indexA - 1] : _vertices[indexA];
        Vector3 pointB = indexB < 0 ? _borderVertices[-indexB - 1] : _vertices[indexB];
        Vector3 pointC = indexC < 0 ? _borderVertices[-indexC - 1] : _vertices[indexC];

        Vector3 sideAB = pointB - pointA;
        Vector3 sideAC = pointC - pointA;

        return Vector3.Cross(sideAB, sideAC).normalized;
    }
}