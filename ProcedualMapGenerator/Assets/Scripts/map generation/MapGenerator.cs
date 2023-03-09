
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// This class generates the an procedural map.
/// </summary>
public class MapGenerator : MonoBehaviour
{
    /// <summary>
    /// The cunk size of the map that will come handy with calculations.
    /// </summary>
    const int mapSize = 241;

    /// <summary>
    /// The collor gradient of the map.
    /// </summary>
    [SerializeField]
    private Gradient gradient;

    /// <summary>
    /// Lods of the map.
    /// </summary>
    [SerializeField]
    [Range(0, 6)]
    private int editorPrevieuwLOD;

    /// <summary>
    /// The number of octaves to use when generating noise map.
    /// </summary>
    [SerializeField]
    private int ocataves;

    /// <summary>
    /// The seed used to generate the noise map.
    /// </summary>
    [SerializeField]
    private int mapSeed;

    /// <summary>
    /// The persistance value used when generating the noise map.
    /// </summary>
    [SerializeField]
    [Range(0, 1)]
    private float presitance;

    /// <summary>
    /// The multiplier used to adjust the height of the mesh.
    /// </summary>
    [SerializeField]
    private float meshHeightMultiplier;

    /// <summary>
    /// The lacunarity value used when generating the noise map.
    /// </summary>
    [SerializeField]
    private float lacunarity;

    /// <summary>
    /// The scale of the noise map.
    /// </summary>
    [SerializeField]
    private float noiseScale;

    /// <summary>
    /// The falloff map used to adjust the edges of the map.
    /// </summary>
    [SerializeField]
    private float[,] fallOffMap;

    /// <summary>
    /// The curve used to adjust the height of the mesh.
    /// </summary>
    [SerializeField]
    private AnimationCurve meshHeightCurve;

    /// <summary>
    /// The offset used to generate the noise map.
    /// </summary>
    [SerializeField]
    private Vector2 offset;

    /// <summary>
    /// Determines whether or not to use the falloff map.
    /// </summary>
    [SerializeField]
    private bool useFallOffs;

    /// <summary>
    /// Determines whether or not to use flat shading for the mesh.
    /// </summary>
    [SerializeField]
    private bool flatShading;

    /// <summary>
    /// The object used to display the generated map.
    /// </summary>
    [SerializeField]
    private GameObject terrainObject;

    /// <summary>
    /// The min terrain height of the generated map
    /// </summary>
    private float minTerrainheight;

    /// <summary>
    /// The max terrain height of the generated map
    /// </summary>
    private float maxTerrainheight;

    /// <summary>
    /// An public call for the generation of the map to be start.
    /// </summary>
    public void StartGenerating()
    {
        GenerateMap();
    }

    /// <summary>
    /// Generates the map with the given specs.
    /// </summary>
    private void GenerateMap()
    {
        UnityEngine.Random.InitState(mapSeed);//seed
        fallOffMap = FalloffGenerator.GenerateFalloffMap(mapSize);
        float[,] noisemap = Noise.GenerateNoiseMap(mapSize, mapSize, mapSeed, noiseScale, ocataves, presitance, lacunarity, offset);

        Color[] collorMap = new Color[mapSize * mapSize];
        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                if (useFallOffs)
                {
                    if (fallOffMap == null)
                    {
                        fallOffMap = FalloffGenerator.GenerateFalloffMap(mapSize);
                    }
                    noisemap[x, y] = Mathf.Clamp01(noisemap[x, y] - fallOffMap[x, y]);
                }
            }
        }

        MapDisplay display = FindObjectOfType<MapDisplay>();
        display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noisemap, meshHeightMultiplier, meshHeightCurve, editorPrevieuwLOD, flatShading, GetComponent<MapGenerator>()), TextureGenerator.TextureFromColourMap(collorMap, mapSize, mapSize));

        StartCoroutine(ColorMap());
    }

    /// <summary>
    /// Checks if values are positive
    /// </summary>
    private void OnValidate()
    {
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (ocataves < 0)
        {
            ocataves = 0;
        }
    }

    /// <summary>
    /// Set min and max height of map for color gradient.
    /// </summary>
    /// <param name="noiseHeight"></param>
    private void SetMinMaxHeights(float noiseHeight)
    {
        if (noiseHeight > maxTerrainheight)
            maxTerrainheight = noiseHeight;
        if (noiseHeight < minTerrainheight)
            minTerrainheight = noiseHeight;
    }

    /// <summary>
    /// Sets the color of the map.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ColorMap()
    {
        yield return new WaitForEndOfFrame();
        Mesh mesh = terrainObject.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        Color[] colors = new Color[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            SetMinMaxHeights(vertices[i].y);
            float height = Mathf.InverseLerp(minTerrainheight, maxTerrainheight, vertices[i].y);
            colors[i] = gradient.Evaluate(height);
            new WaitForSeconds(0.001f);
        }
        mesh.colors = colors;
    }

    /// <summary>
    /// Sets the mapSeed value to the text.
    /// </summary>
    /// <param name="text">text that will set the mapSeed value.</param>
    public void MapSeed(Text text)
    {
        int myInt;

        if (int.TryParse(text.text, out myInt))
        {
            mapSeed = myInt;
        }
        else
        {
            Debug.Log("Unable to parse value");
        }
    }

    /// <summary>
    /// Sets the presitance value to the slider.
    /// </summary>
    /// <param name="slider">Slider that will set the presitance value.</param>
    public void SetPrestance(Slider slider)
    {
        presitance = slider.value;
    }

    /// <summary>
    /// Sets the noiseScale value to the slider.
    /// </summary>
    /// <param name="slider">Slider that will set the noiseScale value.</param>
    public void SetnoiseScale(Slider slider)
    {
        noiseScale = slider.value;
    }
}