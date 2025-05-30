using SoftwareRasterizer.Types;
using static System.MathF;

namespace SoftwareRasterizer.Demo;

public class TerrainGen
{
	const int seed = 42;
	static OpenSimplexNoise noiseGen = new(seed); // thanks to digitalshadow & KdotJPG

	static float CalculateElevation(float2 pos)
	{
		const int layerCount = 5;
		const float lacunarity = 2f; // how fast detail increases per layer
		const float persistence = 0.5f; // how quickly strength of layers decreases
		const int ridgeLayerStart = 3; // make noise more 'ridge-like' in later layers

		float frequency = 0.035f;
		float amplitude = 1;
		float elevation = 0;

		for (int i = 0; i < layerCount; i++)
		{
			float noise = noiseGen.Evaluate(pos.x * frequency, pos.y * frequency);
			if (i >= ridgeLayerStart) noise = 0.5f - Abs(noise);
			elevation += noise * amplitude;
			amplitude *= persistence;
			frequency *= lacunarity;
		}
		
		float v = Maths.Lerp(1, 0.8f, elevation);
		return elevation * Pow(Abs(elevation), 0.5f) * 18 * v;
	}


	static float2 CalculateJiggle(float2 pos)
	{
		const float jiggle = 0.7f;

		float ox = noiseGen.Evaluate(pos.x, pos.y) * jiggle;
		float oy = noiseGen.Evaluate(pos.x - 10000, pos.y - 10000) * jiggle;
		return new float2(ox, oy);
	}

	static float3[,] GeneratePointMap(int resolution, float worldSize, float2 gridCentre)
	{
		float3[,] pointMap = new float3[resolution, resolution];

		for (int y = 0; y < resolution; y++)
		{
			for (int x = 0; x < resolution; x++)
			{
				float2 localGridPos_sNorm = new float2(x, y) / (resolution - 1f) - float2.Half; // [-1, 1]
				float2 gridWorldPos = gridCentre + worldSize * localGridPos_sNorm;
				gridWorldPos += CalculateJiggle(gridWorldPos); // perturb grid slightly for visual intrigue
				float elevation = Max(0, CalculateElevation(gridWorldPos) + 0.8f); // clamp to 0 for water surface
				pointMap[x, y] = new float3(gridWorldPos.x, elevation, gridWorldPos.y);
			}
		}

		return pointMap;
	}

	public static Mesh GenerateTerrain(int resolution, float worldSize, float2 gridCentre)
	{
		float3[,] pointMap = GeneratePointMap(resolution, worldSize, gridCentre);
		MeshData meshData = new();

		for (int y = 0; y < resolution - 1; y++)
		{
			for (int x = 0; x < resolution - 1; x++)
			{
				AddTriangle(pointMap[x, y], pointMap[x, y + 1], pointMap[x + 1, y]); // A, C, B
				AddTriangle(pointMap[x + 1, y], pointMap[x, y + 1], pointMap[x + 1, y + 1]); // B, C, D
			}
		}

		return meshData.ToMesh();

		void AddTriangle(float3 a, float3 b, float3 c)
		{
			float2 texCoord = new float2(a.y + b.y + c.y, 0) / 3;
			float3 surfaceNormal = float3.Cross(b - a, c - b).Normalized();

			meshData.Points.AddRange([a, b, c]);
			meshData.Normals.AddRange([surfaceNormal, surfaceNormal, surfaceNormal]);
			meshData.TexCoords.AddRange([texCoord, texCoord, texCoord]);
		}
	}

	public class MeshData
	{
		public List<float3> Points = new();
		public List<float3> Normals = new();
		public List<float2> TexCoords = new();

		public Mesh ToMesh()
		{
			Mesh mesh = new(Points.ToArray(), Normals.ToArray(), TexCoords.ToArray());
			return mesh;
		}
	}
}