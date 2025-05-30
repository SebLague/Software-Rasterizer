using System.Runtime.CompilerServices;
using SoftwareRasterizer.Types;

namespace SoftwareRasterizer.Demo;

public class CloudShader(float3 directionToLight, float3 tint) : Shader
{
	public float3 DirectionToLight = directionToLight;
	public float3 Tint = tint;
	public float3 AtmosCol;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override float3 PixelColour(float2 pixelCoord, float2 texCoord, float3 normal, float depth)
	{
		normal = float3.Normalize(normal);
		float lightIntensity = (float3.Dot(normal, DirectionToLight) + 1) * 0.5f;
		lightIntensity = Maths.Lerp(0.8f, 1, lightIntensity);

		float t = 1 - MathF.Exp(-depth * 0.0075f);
		float3 col = float3.Lerp(Tint * lightIntensity, AtmosCol, t);
		return col;
	}
}