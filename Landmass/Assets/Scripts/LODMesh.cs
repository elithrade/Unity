using UnityEngine;

// Class responsible for fetching mesh data from MapGenerator.
// Each TerrainChunk has an array of LODMesh objects.
public class LODMesh
{
    public Mesh Mesh;
    public bool HasRequestedMesh;
    public bool HasReceivedMesh;
    private readonly MapGenerator _mapGenerator;
    internal int _lod;

    public LODMesh(MapGenerator mapGenerator, int lod)
    {
        _mapGenerator = mapGenerator;
        _lod = lod;
    }

    public void RequestMesh(MapData mapData)
    {
        _mapGenerator.RequestMeshData(OnMeshDataReceived, _lod, mapData);
        Debug.Log(_lod);
        HasRequestedMesh = true;
    }

    private void OnMeshDataReceived(MeshData meshData)
    {
        Mesh = meshData.CreateMesh();
        HasReceivedMesh = true;
    }
}
