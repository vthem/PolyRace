using UnityEngine;

namespace TSW
{
	public static class FloatArray
	{
		public static void ToPNG(this float[,] field, string filename, bool reverse = false)
		{
			field.ToTexture(reverse).WriteToFile(filename);
		}

		public static Texture2D ToTexture(this float[,] field, bool reverse = false)
		{
			if (field.GetLength(0) != field.GetLength(1))
			{
				throw new System.Exception("The field z length must be equal to x length");
			}
			int textureSize = field.GetLength(0);
			Texture2D texture = new Texture2D(textureSize, textureSize)
			{
				filterMode = FilterMode.Point
			};
			UnityEngine.Color[] pixels = new UnityEngine.Color[texture.width * texture.height];
			for (int z = 0; z < textureSize; z++)
			{
				for (int x = 0; x < textureSize; x++)
				{
					if (reverse)
					{
						pixels[z * textureSize + x] = new UnityEngine.Color(field[z, x], field[z, x], field[z, x], 1f);
					}
					else
					{
						pixels[z * textureSize + x] = new UnityEngine.Color(field[x, z], field[x, z], field[x, z], 1f);
					}
				}
			}
			pixels[0] = UnityEngine.Color.green;
			pixels[pixels.Length - 1] = UnityEngine.Color.red;
			texture.SetPixels(pixels);
			texture.Apply();
			return texture;
		}
	}
}
