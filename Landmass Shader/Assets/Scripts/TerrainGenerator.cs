using System;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public Transform Viewer;
    public static Vector2 ViewerPosition;
    public Material Material;
    public LODInfo[] LevelOfDetails;
    public int ColliderLODIndex;
    public MeshSettings MeshSettings;
    public HeightMapSettings HeightMapSettings;
    public TextureData TextureSettings;

    private float _meshRealWorldSize;
    private int _chunksVisibleInViewDistance;
    private Dictionary<Vector2, TerrainChunk> _terrainChunks;
    private List<TerrainChunk> _visibleChunks = new List<TerrainChunk>();
    internal static float MaxViewDistance;
    private const float _viewerMoveThreshold = 25f;
    private Vector2 _oldViewerPosition;

    private void Start()
    {
        TextureSettings.Apply(Material);
        TextureSettings.SetMinMaxHeight(Material, HeightMapSettings.MinHeight, HeightMapSettings.MaxHeight);

        _terrainChunks = new Dictionary<Vector2, TerrainChunk>();
        _meshRealWorldSize = MeshSettings.MeshWorldSize;

        MaxViewDistance = LevelOfDetails[LevelOfDetails.Length - 1].VisibleThreshold;
        _chunksVisibleInViewDistance = Mathf.RoundToInt(MaxViewDistance / _meshRealWorldSize);

        UpdateVisibleChunks(ViewerPosition);
    }

    private void Update()
    {
        ViewerPosition = new Vector2(Viewer.position.x, Viewer.position.z);
        if (_oldViewerPosition != ViewerPosition)
        {
            foreach (TerrainChunk chunk in _visibleChunks)
                chunk.UpdateCollisionMesh();
        }
        if ((_oldViewerPosition - ViewerPosition).magnitude > _viewerMoveThreshold)
        {
            // Prevent calling UpdateVisibleChunks every frame
            UpdateVisibleChunks(ViewerPosition);
            _oldViewerPosition = ViewerPosition;
        }
    }

    private void UpdateVisibleChunks(Vector2 viewerPosition)
    {
        HashSet<Vector2> alreadyUpdatedChunks = new HashSet<Vector2>();
        for (int i = _visibleChunks.Count - 1; i >= 0; i--)
        {
            // Update will possibly remove chunk from list, iterate from the end to avoid collection changed index error
            alreadyUpdatedChunks.Add(_visibleChunks[i].Coordinate);
            _visibleChunks[i].UpdateTerrainChunk();
        }

        int currentChunkX = Mathf.RoundToInt(viewerPosition.x / _meshRealWorldSize);
        int currentChunkY = Mathf.RoundToInt(viewerPosition.y / _meshRealWorldSize);

        for (int yOffset = -_chunksVisibleInViewDistance; yOffset <= _chunksVisibleInViewDistance; yOffset++)
        {
            for (int xOffset = -_chunksVisibleInViewDistance; xOffset <= _chunksVisibleInViewDistance; xOffset++)
            {
                Vector2 viewedChunkCoordinate = new Vector2(currentChunkX + xOffset, currentChunkY + yOffset);
                if (alreadyUpdatedChunks.Contains(viewedChunkCoordinate))
                    continue;

                if (_terrainChunks.ContainsKey(viewedChunkCoordinate))
                {
                    TerrainChunk viewedChunk = _terrainChunks[viewedChunkCoordinate];
                    viewedChunk.UpdateTerrainChunk();
                }
                else
                {
                    TerrainChunk terrainChunk = new TerrainChunk(transform, Viewer.transform, HeightMapSettings, MeshSettings, ColliderLODIndex, LevelOfDetails, viewedChunkCoordinate, Material);
                    _terrainChunks.Add(viewedChunkCoordinate, terrainChunk);
                    terrainChunk.VisibilityChanged += OnTerrainChunkVisibilityChanged;
                    // Requesting data after subscribe to visibility changed event
                    terrainChunk.RequestData();
                }
            }
        }
    }

    private void OnTerrainChunkVisibilityChanged(TerrainChunk terrainChunk, bool visible)
    {
        if (visible)
            _visibleChunks.Add(terrainChunk);
        else
            _visibleChunks.Remove(terrainChunk);
    }
}
