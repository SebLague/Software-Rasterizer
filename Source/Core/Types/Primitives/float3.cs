using System.Runtime.CompilerServices;
using static System.MathF;

namespace SoftwareRasterizer.Types;

public struct float3(float x, float y, float z)
{
	public float x = x;
	public float y = y;
	public float z = z;

	public float r
	{
		get => x;
		set => x = value;
	}

	public float g
	{
		get => y;
		set => y = value;
	}

	public float b
	{
		get => z;
		set => z = value;
	}

	public static readonly float3 Zero = new float3(0, 0, 0);
	public static readonly float3 One = new float3(1, 1, 1);
	public static readonly float3 Right = new float3(1, 0, 0);
	public static readonly float3 Up = new float3(0, 1, 0);
	public static readonly float3 Forward = new float3(0, 0, 1);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Dot(float3 a, float3 b) => a.x * b.x + a.y * b.y + a.z * b.z;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public float SqrMagnitude() => x * x + y * y + z * z;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public float Magnitude() => Sqrt(SqrMagnitude());

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float3 Normalize(float3 v)
	{
		float sqrLength = Dot(v, v);
		float length = Sqrt(sqrLength);
		if (length == 0) return Zero;
		return v / length;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public float3 Normalized() => Normalize(this);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float3 Cross(float3 a, float3 b)
	{
		float x = (a.y * b.z) - (a.z * b.y);
		float y = (a.z * b.x) - (a.x * b.z);
		float z = (a.x * b.y) - (a.y * b.x);
		return new float3(x, y, z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float3 Lerp(float3 a, float3 b, float t)
	{
		t = Math.Clamp(t, 0, 1);
		return a + (b - a) * t;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float3 operator *(float3 a, float scalar) => new(a.x * scalar, a.y * scalar, a.z * scalar);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float3 operator *(float scalar, float3 a) => a * scalar;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float3 operator /(float3 a, float scalar) => a * (1 / scalar);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float3 operator /(float a, float3 vec) => new(a / vec.x, a / vec.y, a / vec.z);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float3 operator -(float scalar, float3 a) => new(scalar - a.x, scalar - a.y, scalar - a.z);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float3 operator +(float3 a, float3 b) => new(a.x + b.x, a.y + b.y, a.z + b.z);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float3 operator -(float3 a, float3 b) => new(a.x - b.x, a.y - b.y, a.z - b.z);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float3 operator -(float3 a) => new(-a.x, -a.y, -a.z);

	public override string ToString() => $"<{x}, {y}, {z}>";

	public float this[int index]
	{
		get => index switch
		{
			0 => x,
			1 => y,
			2 => z,
			_ => throw new IndexOutOfRangeException("Index must be 0, 1, or 2")
		};
		set
		{
			switch (index)
			{
				case 0: x = value; break;
				case 1: y = value; break;
				case 2: z = value; break;
				default: throw new IndexOutOfRangeException("Index must be 0, 1, or 2");
			}
		}
	}
}