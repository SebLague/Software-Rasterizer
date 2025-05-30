using SoftwareRasterizer.Types;

namespace SoftwareRasterizer.Shaders;

public class TextureShader(Texture texture) : Shader
{
	public Texture Texture = texture;

	public override float3 PixelColour(float2 pixelCoord,float2 texCoord, float3 normal, float depth) => Texture.Sample(texCoord.x, texCoord.y);
}
