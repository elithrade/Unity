using System;
using UnityEngine;

[Serializable]
public struct LODInfo
{
    [Range(0, MeshGenerator.NumberOfSupportedLOD - 1)]
    public int LevelOfDetail;
    // The view distance that determine LOD visibility
    public float VisibleThreshold;

    public float SquaredVisibleThreshold
    {
        get { return VisibleThreshold * VisibleThreshold; }
    }
}