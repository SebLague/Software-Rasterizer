using System.Runtime.CompilerServices;
using SoftwareRasterizer.Types;

namespace SoftwareRasterizer.Shaders;

public class LitTextureShader(float3 directionToLight, Texture texture) : Shader
{
	public float3 DirectionToLight = directionToLight;
	public Texture Texture = texture;
	public float TextureScale = 1;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override float3 PixelColour(float2 pixelCoord,float2 texCoord, float3 normal, float depth)
	{
		normal = float3.Normalize(normal);
		float lightIntensity = (float3.Dot(normal, DirectionToLight) + 1) * 0.5f;
		lightIntensity = Maths.Lerp(0.4f,1,lightIntensity);
		return Texture.Sample(texCoord.x * TextureScale, texCoord.y * TextureScale) * lightIntensity;
	}
}