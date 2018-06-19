using UnityEngine;

public class TerrainChunk
{
    public GameObject _meshObject;
    public Vector2 Coordinate;

    private readonly Vector2 _sampleCentre;
    private readonly int size;
    private Bounds _bounds;
    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;
    private MapGenerator _mapGenerator;
    private readonly int _colliderLODIndex;
    private HeightMap _mapData;
    private int _previousLodIndex = -1;
    private readonly LODInfo[] _levelOfDetails;
    private readonly LODMesh[] _levelOfDetailMeshes;
    private bool _hasSetCollider;

    public TerrainChunk(MapGenerator mapGenerator, int colliderLODIndex, LODInfo[] levelOfDetails, Vector2 coordinate, float meshWorldSize, Material material)
    {
        _mapGenerator = mapGenerator;
        _colliderLODIndex = colliderLODIndex;
        _levelOfDetails = levelOfDetails;
        Coordinate = coordinate;

        _sampleCentre = coordinate * meshWorldSize / mapGenerator.MeshSettings.MeshScale;
        Vector2 position = coordinate * meshWorldSize;
        _bounds = new Bounds(position, Vector2.one * meshWorldSize);

        _meshObject = new GameObject("Terrain Chunk");
        _meshRenderer = _meshObject.AddComponent<MeshRenderer>();
        _meshRenderer.material = material;
        _meshFilter = _meshObject.AddComponent<MeshFilter>();
        _meshCollider = _meshObject.AddComponent<MeshCollider>();

        _meshObject.transform.position =  new Vector3(position.x, 0, position.y);
        _meshObject.transform.parent = mapGenerator.transform;

        SetVisible(false);

        // Each chunk will contain mesh data with different level of detail
        // depends on the view distance LODMesh will request mesh with
        // different level of detail.
        _levelOfDetailMeshes = new LODMesh[_levelOfDetails.Length];
        for (int i = 0; i < _levelOfDetailMeshes.Length; i++)
        {
            _levelOfDetailMeshes[i] = new LODMesh(_mapGenerator, _levelOfDetails[i].LevelOfDetail);
            _levelOfDetailMeshes[i].MeshDataReceived += UpdateTerrainChunk;

            // Make sure update the collision mesh when mesh data received
            // TODO: Unsubscribe event on destroy TerrainChunk
            if (i == _colliderLODIndex)
                _levelOfDetailMeshes[i].MeshDataReceived += UpdateCollisionMesh;
        }

        _mapGenerator.RequestMapData(OnMapDataReceived, _sampleCentre);
    }

    private void OnMapDataReceived(HeightMap height)
    {
        _mapData = height;
        Texture2D texture = TextureGenerator.TextureFromColorMap(
            _mapGenerator.MeshSettings.NumberOfVerticesPerLine,
            _mapGenerator.MeshSettings.NumberOfVerticesPerLine);
        _meshRenderer.material.mainTexture = texture;

        // Update when we received map data
        UpdateTerrainChunk();
    }

    public void UpdateTerrainChunk()
    {
        // Although Mathf.Sqrt is more expensive but we need the
        // viewerDistanceFromNearestEdge to compare against each lod mesh's maximum distance.
        float viewerDistanceFromNearestEdge = Mathf.Sqrt(_bounds.SqrDistance(EndlessTerrain.ViewerPosition));
        bool wasVisible = IsVisible();
        bool visible = viewerDistanceFromNearestEdge <= EndlessTerrain.MaxViewDistance;
        if (visible)
        {
            int lodIndex = 0;
            // Find the correct level of detail index and update the lod mesh
            for (int i = 0; i < _levelOfDetails.Length - 1; i++)
            {
                if (viewerDistanceFromNearestEdge > _levelOfDetails[i].VisibleThreshold)
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
            if (wasVisible != visible)
            {
                if (visible)
                    // Add ourself to visible terrain chunk list since LODMesh can call UpdateTerrainChunk on mesh received
                    EndlessTerrain.VisibleChunks.Add(this);
                else
                    EndlessTerrain.VisibleChunks.Remove(this);

                SetVisible(visible);
            }
        }
    }

    public void UpdateCollisionMesh()
    {
        if (_hasSetCollider)
            return;

        // UpdateCollisionMesh will be called much more frequently
        // then UpdateTerrainChunk. The reason being we want to delay
        // create collider and we need to constantly checking player
        // position, otherwise it will be too late to create collider.
        // This method will be called on each update when player is moved.
        float squareDistanceFromPlayerToEdge = _bounds.SqrDistance(EndlessTerrain.ViewerPosition);
        LODMesh lodMesh = _levelOfDetailMeshes[_colliderLODIndex];
        if (squareDistanceFromPlayerToEdge < _levelOfDetails[_colliderLODIndex].SquaredVisibleThreshold)
        {
            // Start requesting mesh when player is approaching to visible view distance
            if (!lodMesh.HasRequestedMesh)
                lodMesh.RequestMesh(_mapData);
        }
        if (squareDistanceFromPlayerToEdge < EndlessTerrain.ColliderGenerationDistanceThreshold * EndlessTerrain.ColliderGenerationDistanceThreshold)
        {
            // When player is close enough to the edge of current chunk
            if (lodMesh.HasReceivedMesh)
            {
                _meshCollider.sharedMesh = lodMesh.Mesh;
                _hasSetCollider = true;
            }
        }

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