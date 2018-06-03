﻿using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer TextureRenderer;
    public MeshFilter MeshFilter;
    public MeshRenderer MeshRenderer;

    public void DrawTexture(Texture2D texture)
    {
        // TextureRenderer.sharedMaterial will apply material without running the game
        TextureRenderer.sharedMaterial.mainTexture = texture;
        TextureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    internal void DrawMesh(MeshData meshData)
    {
        MeshFilter.sharedMesh = meshData.CreateMesh();
        // Set the mesh size
        MeshFilter.transform.localScale = Vector3.one * FindObjectOfType<MapGenerator>().TerrainData.UniformScale;
    }
}