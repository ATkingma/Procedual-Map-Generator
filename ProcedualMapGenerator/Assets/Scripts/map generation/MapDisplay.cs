using System.Collections;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    /// <summary>
    /// The texture render of the map.
    /// </summary>
    [SerializeField]
    private Renderer textureRenderer;

    /// <summary>
    /// The meshFilther render of the map.
    /// </summary>
    [SerializeField]
    private MeshFilter meshFilther;

    /// <summary>
    /// The meshRenderer render of the map.
    /// </summary>
    [SerializeField]
    private MeshRenderer meshRenderer;

    /// <summary>
    /// Draws the texture of the map.
    /// </summary>
    public void DrawTexture(Texture2D texture)
	{
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
	}

    /// <summary>
    /// Draws the mesh of the map.
    /// </summary>
    public void DrawMesh(MeshData meshData,Texture2D texture)
	{
        meshFilther.sharedMesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;

    }
}