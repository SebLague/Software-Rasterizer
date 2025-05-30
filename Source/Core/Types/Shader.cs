namespace SoftwareRasterizer.Types;

public abstract class Shader
{
	public abstract float3 PixelColour(float2 pixelCoord, float2 texCoord, float3 normal, float depth);
}