using System;
using UnityEngine;

public class TerrainChunk
{
    public const float ColliderGenerationDistanceThreshold = 5f;

    public event Action<TerrainChunk, bool> VisibilityChanged;
    public GameObject _meshObject;
    public Vector2 Coordinate;

    private readonly HeightMapSettings _heightMapSettings;
    private readonly MeshSettings _meshSettings;
    private readonly Vector2 _sampleCentre;
    private readonly int size;
    private Bounds _bounds;
    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;
    private readonly Transform _viewer;
    private readonly int _colliderLODIndex;
    private HeightMap _heightMap;
    private int _previousLodIndex = -1;
    private readonly LODInfo[] _levelOfDetails;
    private readonly LODMesh[] _levelOfDetailMeshes;
    private bool _hasSetCollider;

    public TerrainChunk(Transform parent, Transform viewer, HeightMapSettings heightMapSettings, MeshSettings meshSettings, int colliderLODIndex, LODInfo[] levelOfDetails, Vector2 coordinate, Material material)
    {
        _viewer = viewer;
        _colliderLODIndex = colliderLODIndex;
        _levelOfDetails = levelOfDetails;
        Coordinate = coordinate;
        _heightMapSettings = heightMapSettings;
        _meshSettings = meshSettings;

        float meshWorldSize = _meshSettings.MeshWorldSize;
        _sampleCentre = coordinate * meshWorldSize / _meshSettings.MeshScale;
        Vector2 position = coordinate * meshWorldSize;
        _bounds = new Bounds(position, Vector2.one * meshWorldSize);

        _meshObject = new GameObject("Terrain Chunk");
        _meshRenderer = _meshObject.AddComponent<MeshRenderer>();
        _meshRenderer.material = material;
        _meshFilter = _meshObject.AddComponent<MeshFilter>();
        _meshCollider = _meshObject.AddComponent<MeshCollider>();

        _meshObject.transform.position = new Vector3(position.x, 0, position.y);
        _meshObject.transform.parent = parent;

        SetVisible(false);

        // Each chunk will contain mesh data with different level of detail
        // depends on the view distance LODMesh will request mesh with
        // different level of detail.
        _levelOfDetailMeshes = new LODMesh[_levelOfDetails.Length];
        for (int i = 0; i < _levelOfDetailMeshes.Length; i++)
        {
            _levelOfDetailMeshes[i] = new LODMesh(_levelOfDetails[i].LevelOfDetail);
            _levelOfDetailMeshes[i].MeshDataReceived += UpdateTerrainChunk;

            // Make sure update the collision mesh when mesh data received
            // TODO: Unsubscribe event on destroy TerrainChunk
            if (i == _colliderLODIndex)
                _levelOfDetailMeshes[i].MeshDataReceived += UpdateCollisionMesh;
        }

        RequestData();
    }

    private void OnHeightMapReceived(object heightMapObject)
    {
        _heightMap = (HeightMap)heightMapObject;
        Texture2D texture = TextureGenerator.TextureFromHeightMap(_heightMap);
        _meshRenderer.material.mainTexture = texture;

        // Update when we received map data
        UpdateTerrainChunk();
    }

    internal void RequestData()
    {
        Func<object> generateData = () => HeightMapGenerator.GenerateHeightMap(
            _meshSettings.NumberOfVerticesPerLine,
            _meshSettings.NumberOfVerticesPerLine,
            _heightMapSettings,
            _sampleCentre);

        ThreadedDataRequester.RequestData(generateData, OnHeightMapReceived);
    }

    Vector2 ViewerPosition
    {
        get { return new Vector2(_viewer.position.x, _viewer.position.z); }
    }

    public void UpdateTerrainChunk()
    {
        // Although Mathf.Sqrt is more expensive but we need the
        // viewerDistanceFromNearestEdge to compare against each lod mesh's maximum distance.
        float viewerDistanceFromNearestEdge = Mathf.Sqrt(_bounds.SqrDistance(ViewerPosition));
        bool wasVisible = IsVisible();
        bool visible = viewerDistanceFromNearestEdge <= _levelOfDetails[_levelOfDetails.Length - 1].VisibleThreshold;

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
                    lodMesh.RequestMesh(_heightMap, _meshSettings);
                }
            }
        }
        if (wasVisible != visible)
        {
            SetVisible(visible);

            if (VisibilityChanged != null)
                VisibilityChanged.Invoke(this, visible);
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
        float squareDistanceFromPlayerToEdge = _bounds.SqrDistance(ViewerPosition);
        LODMesh lodMesh = _levelOfDetailMeshes[_colliderLODIndex];
        if (squareDistanceFromPlayerToEdge < _levelOfDetails[_colliderLODIndex].SquaredVisibleThreshold)
        {
            // Start requesting mesh when player is approaching to visible view distance
            if (!lodMesh.HasRequestedMesh)
                lodMesh.RequestMesh(_heightMap, _meshSettings);
        }
        if (squareDistanceFromPlayerToEdge < ColliderGenerationDistanceThreshold * ColliderGenerationDistanceThreshold)
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