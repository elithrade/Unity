using UnityEngine;

public class MapPreview : MonoBehaviour
{
    public Renderer TextureRenderer;
    public MeshFilter MeshFilter;
    public MeshRenderer MeshRenderer;
    public bool AutoUpdate;
    public DrawMode DrawMode;
    [Range(0, MeshSettings.NumberOfSupportedLOD - 1)]
    public int PreviewLOD;
    public MeshSettings MeshSettings;
    public HeightMapSettings HeightMapSettings;
    public TextureData TextureData;
    public Material TextureMaterial;
    private float[,] _falloffMap;

    public void DrawTexture(Texture2D texture)
    {
        // TextureRenderer.sharedMaterial will apply material without running the game
        TextureRenderer.sharedMaterial.mainTexture = texture;
        TextureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height) / 10f;

        TextureRenderer.gameObject.SetActive(true);
        MeshFilter.gameObject.SetActive(false);
    }

    internal void DrawMesh(MeshData meshData)
    {
        MeshFilter.sharedMesh = meshData.CreateMesh();
        // Set the mesh size
        MeshFilter.transform.localScale = Vector3.one * FindObjectOfType<TerrainGenerator>().MeshSettings.MeshScale;

        TextureRenderer.gameObject.SetActive(false);
        MeshFilter.gameObject.SetActive(true);
    }

    public void DrawMap()
    {
        TextureData.SetMinMaxHeight(TextureMaterial, HeightMapSettings.MinHeight, HeightMapSettings.MaxHeight);
        TextureData.Apply(TextureMaterial);

        HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(MeshSettings.NumberOfVerticesPerLine, MeshSettings.NumberOfVerticesPerLine, HeightMapSettings, Vector2.zero);
        if (DrawMode == DrawMode.Noise)
        {
            DrawTexture(TextureGenerator.TextureFromHeightMap(heightMap));
        }
        else if (DrawMode == DrawMode.Mesh)
        {
            DrawMesh(MeshGenerator.Generate(heightMap.Values, MeshSettings, PreviewLOD));
        }
        else if (DrawMode == DrawMode.Falloff)
        {
            DrawTexture(TextureGenerator.TextureFromHeightMap(
                new HeightMap(FalloffGenerator.GenerateFalloffMap(MeshSettings.NumberOfVerticesPerLine),0, 1)));
        }
    }

    private void OnValidate()
    {
        if (MeshSettings != null)
        {
            MeshSettings.OnValueUpdated -= OnValueUpdated;
            MeshSettings.OnValueUpdated += OnValueUpdated;
        }
        if (HeightMapSettings != null)
        {
            HeightMapSettings.OnValueUpdated -= OnValueUpdated;
            HeightMapSettings.OnValueUpdated += OnValueUpdated;
        }
        if (TextureData != null)
        {
            TextureData.OnValueUpdated -= OnTextureUpdated;
            TextureData.OnValueUpdated += OnTextureUpdated;
        }
    }

    private void OnTextureUpdated()
    {
        TextureData.Apply(TextureMaterial);
    }

    private void OnValueUpdated()
    {
        if (Application.isPlaying)
            return;

        DrawMap();
    }
}
