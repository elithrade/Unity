using System;

[Serializable]
public struct LODInfo
{
    public int LevelOfDetail;
    public float MaximumViewDistanceForLevelOfDetail;
    public bool UseForCollider;
}