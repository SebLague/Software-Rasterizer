namespace SoftwareRasterizer.Types;

using System.Runtime.CompilerServices;

public readonly struct float2(float x, float y)
{
	public readonly float x = x;
	public readonly float y = y;

	public static readonly float2 Zero = new(0, 0);
	public static readonly float2 One = new(1, 1);
	public static readonly float2 Half = new(0.5f, 0.5f);
	public static readonly float2 Right = new(1, 0);
	public static readonly float2 Up = new(0, 1);
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Dot(float2 a, float2 b) => a.x * b.x + a.y * b.y;
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float2 Lerp(float2 a, float2 b, float t)
	{
		t = Math.Clamp(t, 0, 1);
		return a + (b - a) * t;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator float2(float3 v) => new(v.x, v.y);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float2 operator *(float2 a, float scalar) => new(a.x * scalar, a.y * scalar);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float2 operator *(float scalar, float2 a) => a * scalar;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float2 operator /(float2 a, float scalar) => a * (1 / scalar);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float2 operator -(float scalar, float2 a) => new(scalar - a.x, scalar - a.y);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float2 operator +(float2 a, float2 b) => new(a.x + b.x, a.y + b.y);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float2 operator -(float2 a, float2 b) => new(a.x - b.x, a.y - b.y);

	public override string ToString() => $"<{x}, {y}>";
}