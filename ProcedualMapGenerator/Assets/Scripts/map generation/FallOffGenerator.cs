using UnityEngine;
public static class FalloffGenerator
{
	/// <summary>
	/// Generates an falloffmap with the given size.
	/// </summary>
	/// <param name="size"></param>
	/// <returns>returns an float map.</returns>
	public static float[,] GenerateFalloffMap(int size)
	{
		float[,] map = new float[size, size];

		for (int i = 0; i < size; i++)
		{
			for (int j = 0; j < size; j++)
			{
				float x = i / (float)size * 2 - 1;
				float y = j / (float)size * 2 - 1;

				float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
				map[i, j] = Evaluate(value);
			}
		}
		return map;
	}

    /// <summary>
    /// This is a static method that takes in a float value as input and returns a float value as output. It is used in the implementation of the Perlin noise algorithm.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    static float Evaluate(float value)
	{
		float a = 3;
		float b = 2.2f;

		return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
	}
}