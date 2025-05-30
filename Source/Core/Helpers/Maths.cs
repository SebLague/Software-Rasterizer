using SoftwareRasterizer.Types;
using System.Runtime.CompilerServices;

namespace SoftwareRasterizer;

public static class Maths
{
	public const float DegreesToRadians = MathF.PI / 180;
	
	// Test if point p is inside triangle ABC
	// Note: non-clockwise triangles are considered 'back-faces' and are ignored
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool PointInTriangle(in float2 a, in float2 b, in float2 c, in float2 p, out float weightA, out float weightB, out float weightC)
	{
		// Test if point is on right side of each edge segment
		float areaABP = SignedParallelogramArea(a, b, p);
		float areaBCP = SignedParallelogramArea(b, c, p);
		float areaCAP = SignedParallelogramArea(c, a, p);
		bool inTri = areaABP >= 0 && areaBCP >= 0 && areaCAP >= 0;

		// Weighting factors (barycentric coordinates)
		float totalArea = (areaABP + areaBCP + areaCAP);
		float invAreaSum = 1 / totalArea;
		weightA = areaBCP * invAreaSum;
		weightB = areaCAP * invAreaSum;
		weightC = areaABP * invAreaSum;

		return inTri && totalArea > 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float SignedParallelogramArea(in float2 a, in float2 b, in float2 c)
	{
		return (c.x - a.x) * (b.y - a.y) + (c.y - a.y) * (a.x - b.x);
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Lerp(float a, float b, float t) => a + (b - a) * Clamp01(t);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Remap01(float value, float min, float max) => Clamp01((value - min) / (max - min));

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Clamp01(float value) => Math.Clamp(value, 0, 1);
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Clamp(float value, float min, float max) => Math.Clamp(value, min, max);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int RoundToInt(float value) => (int)Math.Round(value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float ToRadians(float deg) => deg * DegreesToRadians;
	
}