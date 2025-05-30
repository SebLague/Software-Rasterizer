using SoftwareRasterizer.Types;
using static System.MathF;

namespace SoftwareRasterizer.Demo;

public class TerrainShader(float3 directionToLight) : Shader
{
	public float3 DirectionToLight = directionToLight;
	public float[] Heights = [0, 0.6f, 2.5f, 12f]; // end height for each colour band (not including last)
	public float3 SkyCol;

	public float3[] Colours =
	[
		new(0.2f, 0.6f, 0.98f), // water
		new float3(235f, 205f, 94f) / 255, // sand
		new(0.2f, 0.6f, 0.1f), // grass
		new(0.5f, 0.35f, 0.3f), // mountain
		new(0.93f, 0.93f, 0.91f), // snow
	];

	public override float3 PixelColour(float2 pixelCoord, float2 texCoord, float3 normal, float depth)
	{
		// Get terrain colour from triangle's height
		float triangleHeight = texCoord.x;
		float3 terrainCol = Colours[0];

		for (int i = 0; i < Heights.Length; i++)
		{
			if (triangleHeight > Heights[i]) terrainCol = Colours[i + 1];
			else break;
		}

		// Calculate lighting
		float lightIntensity = (float3.Dot(normal.Normalized(), DirectionToLight) + 1) * 0.5f;
		terrainCol *= lightIntensity;

		// Fade to sky colour in the distance using exponential falloff
		const float atmosphereDensity = 0.0075f;
		float aerialPerspectiveT = 1 - Exp(-depth * atmosphereDensity);
		return float3.Lerp(terrainCol, SkyCol, aerialPerspectiveT);
	}
}