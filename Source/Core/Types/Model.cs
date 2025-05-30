using SoftwareRasterizer.Types;

namespace SoftwareRasterizer;

public class Model(Mesh mesh, Shader shader, string name = "Unnamed")
{
	public readonly string Name = name;
	public readonly Mesh Mesh = mesh;
	public Transform Transform = new();
	public Shader Shader = shader;
	public List<Rasterizer.RasterizerPoint> RasterizerPoints = new();

	public float3[] Vertices => Mesh.Vertices;
	public float3[] Normals => Mesh.Normals;
	public float2[] TexCoords => Mesh.TexCoords;
}