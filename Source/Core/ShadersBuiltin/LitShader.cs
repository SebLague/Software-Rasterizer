using System.Runtime.CompilerServices;
using SoftwareRasterizer.Types;

namespace SoftwareRasterizer.Shaders;

public class LitShader(float3 directionToLight, float3 tint) : Shader
{
	public float3 DirectionToLight = directionToLight;
	public float3 Tint = tint;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override float3 PixelColour(float2 pixelCoord,float2 texCoord, float3 normal, float depth)
	{
		normal = float3.Normalize(normal);
		float lightIntensity = (float3.Dot(normal, DirectionToLight) + 1) * 0.5f;
		lightIntensity = Maths.Lerp(0.1f,1,lightIntensity);
		return Tint * lightIntensity;
	}
}