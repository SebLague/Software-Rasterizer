using System.Runtime.CompilerServices;

namespace SoftwareRasterizer.Types;

public class Texture(float3[,] image)
{
	public readonly int Width = image.GetLength(0);
	public readonly int Height = image.GetLength(1);
	
	readonly int wscale = image.GetLength(0) - 1;
	readonly int hscale = image.GetLength(1) - 1;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public float3 Sample(float u, float v)
	{
		// Calculate indices of nearest texel to sample point
		int x = (int)((u - MathF.Floor(u)) * wscale);
		int y = (int)((v - MathF.Floor(v)) * hscale);

		return image[x, y];
	}


	public static Texture CreateFromBytes(byte[] bytes)
	{
		int width = bytes[0] | (bytes[1] << 8);
		int height = bytes[2] | (bytes[3] << 8);

		float3[,] image = new float3[width, height];
		int index = 0;

		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				float r = bytes[index + 1] / 255f;
				float g = bytes[index + 2] / 255f;
				float b = bytes[index + 0] / 255f;
				index += 3;
				image[x, y] = new float3(r, g, b);
			}
		}

		return new Texture(image);
	}
}