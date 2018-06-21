using System;
using UnityEngine;

// Class responsible for fetching mesh data from MapGenerator.
// Each TerrainChunk has an array of LODMesh objects.
public class LODMesh
{
    public Mesh Mesh;
    public bool HasRequestedMesh;
    public bool HasReceivedMesh;
    public event Action MeshDataReceived;

    private int _lod;

    public LODMesh(int lod)
    {
        _lod = lod;
    }

    public void RequestMesh(HeightMap heightMap, MeshSettings meshSettings)
    {
        ThreadedDataRequester.RequestData(() => MeshGenerator.Generate(heightMap.Values, meshSettings, _lod), OnMeshDataReceived);
        HasRequestedMesh = true;
    }

    private void OnMeshDataReceived(object meshDataObject)
    {
        Mesh = ((MeshData) meshDataObject).CreateMesh();
        HasReceivedMesh = true;

        MeshDataReceived();
    }
}
