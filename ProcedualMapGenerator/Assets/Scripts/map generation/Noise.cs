using UnityEngine;
using System.Collections;
/// <summary>
/// Generates an noise map.
/// </summary>
public static class Noise
{
    /// <summary>
    /// Generates a 2D noise map using Perlin noise.
    /// </summary>
    /// <param name="mapWidth">Width of the noise map.</param>
    /// <param name="mapHeight">Height of the noise map.</param>
    /// <param name="seed">Seed value for the random number generator.</param>
    /// <param name="scale">Scale of the noise map.</param>
    /// <param name="octaves">Number of octaves used in the Perlin noise algorithm.</param>
    /// <param name="persistence">Controls how much each octave contributes to the overall shape of the noise.</param>
    /// <param name="lacunarity">Controls how much the frequency of each octave changes.</param>
    /// <param name="offset">Offset applied to each octave.</param>
    /// <returns>A 2D float array representing the noise map.</returns>
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight,int seed, float scale , int octaves,float presistance,float lacunarity,Vector2 offset)
	{
		float[,] noiseMap = new float[mapWidth, mapHeight];

		System.Random prng = new System.Random(seed);
		Vector2[] octaveOffsets = new Vector2[octaves];
		for(int i =0;i< octaves; i++)
		{
			float offsetX = prng.Next(-100000,100000)+ offset.x;
			float offsetY = prng.Next(-100000, 100000)+ offset.y;
			octaveOffsets[i] = new Vector2(offsetX, offsetY);
		}

		if (scale <= 0)
		{
			scale = 0.0001f;
		}
		float maxNoiseHeight = float.MinValue;
		float minNoiseHeight = float.MaxValue;

		float halfWidth = mapWidth / 2f;
		float halfHeight = mapHeight / 2f;

		for (int y = 0; y < mapHeight; y++)
		{
			for (int x = 0; x < mapWidth; x++)
			{
				float amplitudes = 1;
				float frequency = 1;
				float noiseHeight = 0;
				for (int i = 0; i < octaves; i++)
				{
					float sampleX = (x-halfWidth) / scale*frequency + octaveOffsets[i].x;
					float sampleY = (y - halfHeight) / scale*frequency + octaveOffsets[i].y;

					float perlinValue = Mathf.PerlinNoise(sampleX, sampleY)*2-1;
					noiseHeight += perlinValue * amplitudes;

					amplitudes *= presistance;
					frequency *= lacunarity;
				}
				if (noiseHeight > maxNoiseHeight)
				{
					maxNoiseHeight = noiseHeight;
				}
				else if (noiseHeight < minNoiseHeight)
				{
					minNoiseHeight = noiseHeight;
				}
				noiseMap[x, y] = noiseHeight;
			}
		}
		for (int y = 0; y < mapHeight; y++)
		{
			for (int x = 0; x < mapWidth; x++)
			{
				noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
			}
		}
				return noiseMap;
	}
}