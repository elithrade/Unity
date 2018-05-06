using System;
using UnityEngine;

public class TerrainChunk
{
    private readonly Vector2 _position;
    private readonly int size;
    private Bounds _bounds;
    public GameObject _meshObject;

    public TerrainChunk(Transform parent, Vector2 coordinate, int size)
    {
        _position = coordinate * size;
        _bounds = new Bounds(_position, Vector2.one * size);
        _meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        _meshObject.transform.position = new Vector3(_position.x, 0, _position.y);
        _meshObject.transform.localScale = Vector3.one * size / 10f;
        _meshObject.transform.parent = parent;

        SetVisible(false);
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