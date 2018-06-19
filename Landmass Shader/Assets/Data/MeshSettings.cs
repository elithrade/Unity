using UnityEngine;

[CreateAssetMenu()]
public class MeshSettings : UpdatableData
{
    public bool UseFlatShading;
    // Scales on the x, y, and z axis
    public float MeshScale = 2.5f;

    // LOD from 0 to 4
    public const int NumberOfSupportedLOD = 5;
    public const int NumberOfSupportedChunkSizes = 9;
    public const int NumberOfSupportedFlatshadedChunkSizes = 3;
    // For i = 0; i < 241; i++
    // If i % 2 == 0
    public static readonly int[] SupportedChunkSizes = {48, 72, 96, 120, 144, 168, 192, 216, 240};

    [Range(0, NumberOfSupportedChunkSizes - 1)]
    public int ChunkSizeIndex;
    [Range(0, NumberOfSupportedFlatshadedChunkSizes - 1)]
    public int FlatshadedChunkSizeIndex;

    // Number of vertices per line of mesh rendered at the
    // highest LOD which is 0.
    // Includes the extra 2 vertices that are excluded from
    // the final mesh, but used for calculating normals.
    public int NumberOfVerticesPerLine
    {
        get
        {
            return SupportedChunkSizes[UseFlatShading ? FlatshadedChunkSizeIndex : ChunkSizeIndex] + 1;
        }
    }

    public float MeshWorldSize
    {
        get{
            return (NumberOfVerticesPerLine - 1 - 2) * MeshScale;
        }
    }
}
