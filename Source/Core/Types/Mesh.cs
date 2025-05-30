namespace SoftwareRasterizer.Types;

public class Mesh(float3[] vertices, float3[] normals, float2[] texCoords)
{
	public readonly float3[] Vertices = vertices;
	public readonly float3[] Normals = normals;
	public readonly float2[] TexCoords = texCoords;
}