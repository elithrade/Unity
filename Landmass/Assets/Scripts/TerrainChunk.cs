using System;
using UnityEngine;

public class TerrainChunk
{
    public GameObject _meshObject;

    private readonly Vector2 _position;
    private readonly int size;
    private Bounds _bounds;
    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;
    private MapGenerator _mapGenerator;

    public TerrainChunk(MapGenerator mapGenerator, Vector2 coordinate, int size, Material material)
    {
        _position = coordinate * size;
        _bounds = new Bounds(_position, Vector2.one * size);

        _meshObject = new GameObject("Terrain Chunk");
        _meshRenderer = _meshObject.AddComponent<MeshRenderer>();
        _meshRenderer.material = material;
        _meshFilter = _meshObject.AddComponent<MeshFilter>();

        _meshObject.transform.position = new Vector3(_position.x, 0, _position.y);
        _meshObject.transform.parent = mapGenerator.transform;

        SetVisible(false);

        _mapGenerator = mapGenerator;
        _mapGenerator.RequestMapData(OnMapDataReceived);
    }

    private void OnMapDataReceived(MapData mapData)
    {
        _mapGenerator.RequestMeshData(OnMeshDataReceived, mapData);
    }

    private void OnMeshDataReceived(MeshData meshData)
    {
        _meshFilter.mesh = meshData.CreateMesh();
    }

    public void UpdateChunk(Vector2 viewerPosition)
    {
        float viewerDistanceFromNearestEdge = _bounds.SqrDistance(viewerPosition);
        bool visible = viewerDistanceFromNearestEdge <= EndlessTerrain.MaxViewDistance * EndlessTerrain.MaxViewDistance;
        SetVisible(visible);
    }

    public void SetVisible(bool visible)
    {
        _meshObject.SetActive(visible);
    }

    public bool IsVisible()
    {
        return _meshObject.activeSelf;
    }
}