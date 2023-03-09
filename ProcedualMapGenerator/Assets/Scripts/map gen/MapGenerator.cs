using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MapGenerator : MonoBehaviour
{
    const int mapSize = 241;

    [SerializeField]
    private Gradient gradient;
    [SerializeField]
    [Range(0, 6)]
    private int editorPrevieuwLOD;
    [SerializeField]
    private int ocataves;
    [SerializeField]
    private int mapSeed;
    [Range(0, 1)]
    public float presitance;
    public float meshHeightMultiplier;
    public float lacunarity;
    public float noiseScale;
    public float[,] fallOffMap;
    public AnimationCurve meshHeightCurve;
    public Vector2 offset;
    public bool useFallOffs;
    public bool generateEnviroment;
    public bool flatShading;
    public GameObject terrainObject;
    private float minTerrainheight;
    private float maxTerrainheight;
    
    public void StartGenerating()
	{
        GenerateMap();
    }
    private void GenerateMap()
    {
        Random.InitState(mapSeed);//seed
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
        display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noisemap, meshHeightMultiplier, meshHeightCurve, editorPrevieuwLOD, flatShading,GetComponent<MapGenerator>()), TextureGenerator.TextureFromColourMap(collorMap, mapSize, mapSize));

        StartCoroutine(ColorMap());
    }

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

    private void SetMinMaxHeights(float noiseHeight)
    {
        // Set min and max height of map for color gradient
        if (noiseHeight > maxTerrainheight)
            maxTerrainheight = noiseHeight;
        if (noiseHeight < minTerrainheight)
            minTerrainheight = noiseHeight;
    }

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
}