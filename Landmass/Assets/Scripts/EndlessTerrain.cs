using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    public const float MaxViewDistance = 450;
    public Transform Viewer;
    public static Vector2 ViewerPosition;

    private int _chunkSize;
    private int _chunksVisibleInViewDistance;
    private Dictionary<Vector2, TerrainChunk> _terrainChunks;
    private List<TerrainChunk> _VisibleTerrainChunksSinceLastUpdate;

    private void Start()
    {
        _terrainChunks = new Dictionary<Vector2, TerrainChunk>();
        _VisibleTerrainChunksSinceLastUpdate = new List<TerrainChunk>();
        _chunkSize = MapGenerator.MeshChunkSize - 1;
        _chunksVisibleInViewDistance = Mathf.RoundToInt(MaxViewDistance / _chunkSize);
    }

    private void Update()
    {
        ViewerPosition = new Vector2(Viewer.position.x, Viewer.position.z);
        UpdateVisibleChunks(ViewerPosition);
    }

    private void UpdateVisibleChunks(Vector2 viewerPosition)
    {
        for (int i = 0; i < _VisibleTerrainChunksSinceLastUpdate.Count; i++)
        {
            _VisibleTerrainChunksSinceLastUpdate[i].SetVisible(false);
        }
        _VisibleTerrainChunksSinceLastUpdate.Clear();

        int currentChunkX = Mathf.RoundToInt(viewerPosition.x / _chunkSize);
        int currentChunkY = Mathf.RoundToInt(viewerPosition.y / _chunkSize);

        for (int yOffset = -_chunksVisibleInViewDistance; yOffset <= _chunksVisibleInViewDistance; yOffset++)
        {
            for (int xOffset = -_chunksVisibleInViewDistance; xOffset <= _chunksVisibleInViewDistance; xOffset++)
            {
                Vector2 viewedChunkCoordinate = new Vector2(currentChunkX + xOffset, currentChunkY + yOffset);
                if (_terrainChunks.ContainsKey(viewedChunkCoordinate))
                {
                    TerrainChunk viewedChunk = _terrainChunks[viewedChunkCoordinate];
                    viewedChunk.UpdateChunk(viewerPosition);
                    if (viewedChunk.IsVisible())
                        _VisibleTerrainChunksSinceLastUpdate.Add(viewedChunk);
                }
                else
                {
                    _terrainChunks.Add(viewedChunkCoordinate, new TerrainChunk(transform, viewedChunkCoordinate, _chunkSize));
                }
            }
        }
    }
}
