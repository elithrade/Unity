using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer TextureRenderer;

    public void Draw(Texture2D texture)
    {
        // TextureRenderer.sharedMaterial will apply material without running the game
        TextureRenderer.sharedMaterial.mainTexture = texture;
        TextureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }
}
