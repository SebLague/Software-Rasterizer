using SoftwareRasterizer.Types;
using static System.Math;

namespace SoftwareRasterizer;

public static class Rasterizer
{
	static Camera cam;
	static RenderTarget target;


	public static void Render(RenderTarget target, SceneData data)
	{
		Rasterizer.target = target;
		Rasterizer.cam = data.Camera;

		Parallel.ForEach(data.Models, ProcessModel);

		foreach (Model model in data.Models)
		{
			int triCount = model.RasterizerPoints.Count / 3;

			Parallel.For(0, triCount, n =>
			{
				int i = n * 3;
				RasterizerPoint r0 = model.RasterizerPoints[i + 0];
				RasterizerPoint r1 = model.RasterizerPoints[i + 1];
				RasterizerPoint r2 = model.RasterizerPoints[i + 2];

				float2 a = r0.ScreenPos;
				float2 b = r1.ScreenPos;
				float2 c = r2.ScreenPos;

				// Triangle bounds
				float minX = Min(Min(a.x, b.x), c.x);
				float minY = Min(Min(a.y, b.y), c.y);
				float maxX = Max(Max(a.x, b.x), c.x);
				float maxY = Max(Max(a.y, b.y), c.y);
				// Pixel block covering the triangle bounds
				int blockStartX = Clamp((int)(minX), 0, target.Width - 1);
				int blockStartY = Clamp((int)(minY), 0, target.Height - 1);
				int blockEndX = Clamp(CeilInt(maxX), 0, target.Width - 1);
				int blockEndY = Clamp(CeilInt(maxY), 0, target.Height - 1);

				float3 invDepths = new(1 / r0.Depth, 1 / r1.Depth, 1 / r2.Depth);
				float2 tx = r0.TexCoords * invDepths[0];
				float2 ty = r1.TexCoords * invDepths[1];
				float2 tz = r2.TexCoords * invDepths[2];
				float3 nx = r0.Normals * invDepths[0];
				float3 ny = r1.Normals * invDepths[1];
				float3 nz = r2.Normals * invDepths[2];

				// Loop over the block of pixels covering the triangle bounds
				for (int y = blockStartY; y <= blockEndY; y++)
				{
					for (int x = blockStartX; x <= blockEndX; x++)
					{
						float2 p = new(x, y);
						if (Maths.PointInTriangle(a, b, c, p, out float weightA, out float weightB, out float weightC))
						{
							// Interpolate depths at each vertex to get value for current pixel
							float depth = 1 / (invDepths.x * weightA + invDepths.y * weightB + invDepths.z * weightC);

							// Depth test (skip if something nearer has already been drawn)
							int px = y * target.Width + x;
							if (depth >= target.DepthBuffer[px]) continue;

							// Interpolate texture coordinates at each vertex
							float2 texCoord = (tx * weightA + ty * weightB + tz * weightC) * depth;
							float3 normal = (nx * weightA + ny * weightB + nz * weightC) * depth;
							float3 col = model.Shader.PixelColour(p, texCoord, normal, depth);

							// Thread-safe pixel update
							lock (target.locks[px])
							{
								if (depth >= target.DepthBuffer[px]) continue;
								target.ColourBuffer[px] = col;
								target.DepthBuffer[px] = depth;
							}
						}
					}
				}
			});
		}
	}

	// Create list of rasterization points for rendering the given model
	static void ProcessModel(Model model)
	{
		Span<float3> viewPoints = stackalloc float3[3];
		model.RasterizerPoints.Clear();

		for (int i = 0; i < model.Vertices.Length; i += 3)
		{
			viewPoints[0] = VertexToView(model.Vertices[i + 0], model.Transform);
			viewPoints[1] = VertexToView(model.Vertices[i + 1], model.Transform);
			viewPoints[2] = VertexToView(model.Vertices[i + 2], model.Transform);

			// Dividing by depths too close to zero causes numerical issues,
			// so use some small positive value for the depth clip threshold
			const float nearClipDst = 0.01f;
			bool clip0 = viewPoints[0].z <= nearClipDst;
			bool clip1 = viewPoints[1].z <= nearClipDst;
			bool clip2 = viewPoints[2].z <= nearClipDst;
			int clipCount = BoolToInt(clip0) + BoolToInt(clip1) + BoolToInt(clip2);

			switch (clipCount)
			{
				case 0:
					AddRasterizerPoint(model, viewPoints[0], i + 0);
					AddRasterizerPoint(model, viewPoints[1], i + 1);
					AddRasterizerPoint(model, viewPoints[2], i + 2);
					break;
				case 1:
				{
					// Figure out which point is to be clipped, and the two that will remain
					int indexClip = clip0 ? 0 : clip1 ? 1 : 2;
					int indexNext = (indexClip + 1) % 3;
					int indexPrev = (indexClip - 1 + 3) % 3;
					float3 pointClipped = viewPoints[indexClip];
					float3 pointA = viewPoints[indexNext];
					float3 pointB = viewPoints[indexPrev];

					// Fraction along triangle edge at which the depth is equal to the clip distance
					float fracA = (nearClipDst - pointClipped.z) / (pointA.z - pointClipped.z);
					float fracB = (nearClipDst - pointClipped.z) / (pointB.z - pointClipped.z);

					// New triangle points (in view space)
					float3 clipPointAlongEdgeA = float3.Lerp(pointClipped, pointA, fracA);
					float3 clipPointAlongEdgeB = float3.Lerp(pointClipped, pointB, fracB);

					// Create new triangles
					AddRasterizerPoint(model, clipPointAlongEdgeB, i + indexClip, i + indexPrev, fracB);
					AddRasterizerPoint(model, clipPointAlongEdgeA, i + indexClip, i + indexNext, fracA);
					AddRasterizerPoint(model, pointB, i + indexPrev);

					AddRasterizerPoint(model, clipPointAlongEdgeA, i + indexClip, i + indexNext, fracA);
					AddRasterizerPoint(model, pointA, i + indexNext);
					AddRasterizerPoint(model, pointB, i + indexPrev);
					break;
				}
				case 2:
				{
					// Figure out which point will not be clipped, and the two that will be
					int indexNonClip = !clip0 ? 0 : !clip1 ? 1 : 2;
					int indexNext = (indexNonClip + 1) % 3;
					int indexPrev = (indexNonClip - 1 + 3) % 3;

					float3 pointNotClipped = viewPoints[indexNonClip];
					float3 pointA = viewPoints[indexNext];
					float3 pointB = viewPoints[indexPrev];

					// Fraction along triangle edge at which the depth is equal to the clip distance
					float fracA = (nearClipDst - pointNotClipped.z) / (pointA.z - pointNotClipped.z);
					float fracB = (nearClipDst - pointNotClipped.z) / (pointB.z - pointNotClipped.z);

					// New triangle points (in view space)
					float3 clipPointAlongEdgeA = float3.Lerp(pointNotClipped, pointA, fracA);
					float3 clipPointAlongEdgeB = float3.Lerp(pointNotClipped, pointB, fracB);

					// Create new triangle
					AddRasterizerPoint(model, clipPointAlongEdgeB, i + indexNonClip, i + indexPrev, fracB);
					AddRasterizerPoint(model, pointNotClipped, i + indexNonClip);
					AddRasterizerPoint(model, clipPointAlongEdgeA, i + indexNonClip, i + indexNext, fracA);
					break;
				}
			}
		}
	}

	static void AddRasterizerPoint(Model model, float3 viewPoint, int vertIndex)
	{
		model.RasterizerPoints.Add(new RasterizerPoint()
		{
			Depth = viewPoint.z,
			ScreenPos = ViewToScreen(viewPoint),
			TexCoords = model.TexCoords[vertIndex],
			Normals = model.Normals[vertIndex],
		});
	}

	static void AddRasterizerPoint(Model model, float3 viewPoint, int vertIndexA, int vertIndexB, float t)
	{
		model.RasterizerPoints.Add(new RasterizerPoint()
		{
			Depth = viewPoint.z,
			ScreenPos = ViewToScreen(viewPoint),
			TexCoords = float2.Lerp(model.TexCoords[vertIndexA], model.TexCoords[vertIndexB], t),
			Normals = float3.Lerp(model.Normals[vertIndexA], model.Normals[vertIndexB], t),
		});
	}

	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	static int BoolToInt(bool b) => b ? 1 : 0;

	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	static float3 VertexToView(float3 vert, Transform transform)
	{
		float3 vertex_world = transform.ToWorldPoint(vert);
		float3 vertex_view = cam.Transform.ToLocalPoint(vertex_world);
		return vertex_view;
	}

	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	static float2 ViewToScreen(float3 vertex_view)
	{
		float screenHeight_world = MathF.Tan(cam.Fov / 2) * 2;
		float pixelsPerWorldUnit = target.Size.y / screenHeight_world / vertex_view.z;

		float2 pixelOffset = new float2(vertex_view.x, vertex_view.y) * pixelsPerWorldUnit;
		float2 vertex_screen = target.Size / 2f + pixelOffset;
		return vertex_screen;
	}

	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	static int CeilInt(float x) => (int)Ceiling(x);

	public struct RasterizerPoint
	{
		public float Depth;
		public float2 ScreenPos;
		public float2 TexCoords;
		public float3 Normals;
	}
}