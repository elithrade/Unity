using UnityEngine;

public class MapData
{
    public readonly float[,] HeightMap;

    public MapData(float[,] heightMap)
    {
        HeightMap = heightMap;
    }
}