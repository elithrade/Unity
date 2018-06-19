using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    public Transform Viewer;
    public static Vector2 ViewerPosition;
    public Material Material;
    public LODInfo[] LevelOfDetails;
    public int ColliderLODIndex;
    public const float ColliderGenerationDistanceThreshold = 5f;

    private float _meshRealWorldSize;
    private int _chunksVisibleInViewDistance;
    private MapGenerator _mapGenerator;
    private Dictionary<Vector2, TerrainChunk> _terrainChunks;
    public static List<TerrainChunk> VisibleChunks = new List<TerrainChunk>();
    internal static float MaxViewDistance;
    private const float _viewerMoveThreshold = 25f;
    private Vector2 _oldViewerPosition;

    private void Start()
    {
        _mapGenerator = FindObjectOfType<MapGenerator>();
        _terrainChunks = new Dictionary<Vector2, TerrainChunk>();
        _meshRealWorldSize = _mapGenerator.MeshSettings.MeshWorldSize;

        MaxViewDistance = LevelOfDetails[LevelOfDetails.Length - 1].VisibleThreshold;
        _chunksVisibleInViewDistance = Mathf.RoundToInt(MaxViewDistance / _meshRealWorldSize);

        UpdateVisibleChunks(ViewerPosition);
    }

    private void Update()
    {
        ViewerPosition = new Vector2(Viewer.position.x, Viewer.position.z);
        if (_oldViewerPosition != ViewerPosition)
        {
            foreach (TerrainChunk chunk in VisibleChunks)
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
        for (int i = VisibleChunks.Count - 1; i >= 0; i--)
        {
            // Update will possibly remove chunk from list, iterate from the end to avoid collection changed index error
            VisibleChunks[i].UpdateTerrainChunk();
            alreadyUpdatedChunks.Add(VisibleChunks[i].Coordinate);
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
                    _terrainChunks.Add(viewedChunkCoordinate, new TerrainChunk(_mapGenerator, ColliderLODIndex, LevelOfDetails, viewedChunkCoordinate, _meshRealWorldSize, Material));
                }
            }
        }
    }
}
