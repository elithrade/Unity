using UnityEngine;

public class TerrainChunk
{
    public GameObject _meshObject;

    public static float Scale = 2f;

    private readonly Vector2 _position;
    private readonly int size;
    private Bounds _bounds;
    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;
    private MapGenerator _mapGenerator;
    private MapData _mapData;
    private int _previousLodIndex = -1;
    private readonly LODInfo[] _levelOfDetails;
    private readonly LODMesh[] _levelOfDetailMeshes;
    private LODMesh _colliderMesh;

    public TerrainChunk(MapGenerator mapGenerator, LODInfo[] levelOfDetails, Vector2 coordinate, int size, Material material)
    {
        _position = coordinate * size;
        _bounds = new Bounds(_position, Vector2.one * size);

        _meshObject = new GameObject("Terrain Chunk");
        _meshRenderer = _meshObject.AddComponent<MeshRenderer>();
        _meshRenderer.material = material;
        _meshFilter = _meshObject.AddComponent<MeshFilter>();
        _meshCollider = _meshObject.AddComponent<MeshCollider>();

        _meshObject.transform.position = new Vector3(_position.x, 0, _position.y) * Scale;
        _meshObject.transform.parent = mapGenerator.transform;
        _meshObject.transform.localScale = Vector3.one * Scale;

        SetVisible(false);

        _mapGenerator = mapGenerator;
        _levelOfDetails = levelOfDetails;

        // Each chunk will contain mesh data with different level of detail
        // depends on the view distance LODMesh will request mesh with
        // different level of detail.
        _levelOfDetailMeshes = new LODMesh[_levelOfDetails.Length];
        for (int i = 0; i < _levelOfDetailMeshes.Length; i++)
        {
            // Pass in callback update when we received mesh data
            _levelOfDetailMeshes[i] = new LODMesh(_mapGenerator, _levelOfDetails[i].LevelOfDetail, UpdateTerrainChunk);
            if (_levelOfDetails[i].UseForCollider)
                _colliderMesh = _levelOfDetailMeshes[i];
        }

        _mapGenerator.RequestMapData(OnMapDataReceived, _position);
    }

    private void OnMapDataReceived(MapData mapData)
    {
        _mapData = mapData;
        Texture2D texture = TextureGenerator.TextureFromColorMap(mapData.ColorMap, _mapGenerator.MeshChunkSize, _mapGenerator.MeshChunkSize);
        _meshRenderer.material.mainTexture = texture;

        // Update when we received map data
        UpdateTerrainChunk();
    }

    public void UpdateTerrainChunk()
    {
        if (_mapData == null)
            return;

        // Although Mathf.Sqrt is more expensive but we need the
        // viewerDistanceFromNearestEdge to compare against each lod mesh's maximum distance.
        float viewerDistanceFromNearestEdge = Mathf.Sqrt(_bounds.SqrDistance(EndlessTerrain.ViewerPosition));
        bool visible = viewerDistanceFromNearestEdge <= EndlessTerrain.MaxViewDistance;
        if (visible)
        {
            int lodIndex = 0;
            // Find the correct level of detail index and update the lod mesh
            for (int i = 0; i < _levelOfDetails.Length - 1; i++)
            {
                if (viewerDistanceFromNearestEdge > _levelOfDetails[i].MaximumViewDistanceForLevelOfDetail)
                    lodIndex = i + 1;
                else
                    break;
            }
            if (lodIndex != _previousLodIndex)
            {
                // If the lod index has changed
                LODMesh lodMesh = _levelOfDetailMeshes[lodIndex];
                if (lodMesh.HasReceivedMesh)
                {
                    _meshFilter.mesh = lodMesh.Mesh;
                    _previousLodIndex = lodIndex;
                }
                else if (!lodMesh.HasRequestedMesh)
                {
                    lodMesh.RequestMesh(_mapData);
                }
            }

            // Only add collider if close enough
            if (lodIndex == 0)
            {
                if (_colliderMesh.HasReceivedMesh)
                    _meshCollider.sharedMesh = _colliderMesh.Mesh;
                else if (!_colliderMesh.HasRequestedMesh)
                    _colliderMesh.RequestMesh(_mapData);
            }

            // Add ourself to visible terrain chunk list since LODMesh can call UpdateTerrainChunk on mesh received
            EndlessTerrain.VisibleTerrainChunksSinceLastUpdate.Add(this);
        }

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