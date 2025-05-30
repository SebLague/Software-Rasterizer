using SoftwareRasterizer.Shaders;
using SoftwareRasterizer.Types;

namespace SoftwareRasterizer.Helpers;

public static class ResourceHelper
{
	public static Model LoadModel(string name, Shader shader = null)
	{
		string objPath = GetResourcesPath("Models", name + ".obj");
		string objString = File.ReadAllText(objPath);
		Mesh mesh = ObjLoader.Load(objString);
		Model model = new(mesh, shader ?? new LitShader(float3.Up, float3.One), name);
		return model;
	}

	public static Texture LoadTexture(string textureName) => Texture.CreateFromBytes(GetTextureBytes(textureName));

	public static byte[] GetTextureBytes(string textureName)
	{
		string texturePath = GetResourcesPath("Textures", textureName + ".bytes");
		return File.ReadAllBytes(texturePath);
	}

	static string GetResourcesPath(string directory, string file)
	{
		return Path.Combine(Directory.GetCurrentDirectory(), "Resources", directory, file);
	}
}