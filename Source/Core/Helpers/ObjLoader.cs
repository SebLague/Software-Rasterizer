using SoftwareRasterizer.Types;
using System.Globalization;

namespace SoftwareRasterizer.Helpers;

public static class ObjLoader
{
	static readonly string[] newLineStrings = ["\r\n", "\r", "\n"];
	static readonly CultureInfo culture = CultureInfo.InvariantCulture;

	// Highly inefficient and incomplete obj parser
	public static Mesh Load(string modelString)
	{
		string[] lines = SplitByLine(modelString, true);

		List<float3> vertexPositions = new();
		List<float3> normals = new();
		List<float2> texCoords = new();
		List<VertexData> allVertexData = new();

		foreach (string line in lines)
		{
			// ---- Vertex position ----
			if (line.StartsWith("v "))
			{
				string[] axisStrings = line[2..].Split(' ');
				float3 v = new(float.Parse(axisStrings[0], culture), float.Parse(axisStrings[1], culture), float.Parse(axisStrings[2], culture));
				vertexPositions.Add(v);
			}
			// ---- Vertex normal ----
			else if (line.StartsWith("vn "))
			{
				string[] axisStrings = line[3..].Split(' ');
				float3 v = new(float.Parse(axisStrings[0], culture), float.Parse(axisStrings[1], culture), float.Parse(axisStrings[2], culture));
				normals.Add(v);
			}
			else if (line.StartsWith("vt "))
			{
				string[] axisStrings = line[3..].Split(' ');
				float2 t = new(float.Parse(axisStrings[0], culture), float.Parse(axisStrings[1], culture));
				texCoords.Add(t);
			}
			// ---- Face Indices ----
			else if (line.StartsWith("f "))
			{
				string[] faceGroupStrings = line[2..].Split(' ');
				for (int i = 0; i < faceGroupStrings.Length; i++)
				{
					string[] faceEntryStrings = faceGroupStrings[i].Split('/');
					bool hasVertIndex = int.TryParse(faceEntryStrings[0], culture, out int vertexIndex);
					bool hasTexIndex = int.TryParse(faceEntryStrings[1], culture, out int tCoordIndex);
					bool hasNormalIndex = int.TryParse(faceEntryStrings[2], culture, out int normalIndex);

					VertexData vert = new()
					{
						Position = hasVertIndex ? vertexPositions[vertexIndex - 1] : float3.Zero,
						Normal = hasNormalIndex ? normals[normalIndex - 1] : float3.Zero,
						TexCoord = hasTexIndex ? texCoords[tCoordIndex - 1] : float2.Zero,
					};
					if (i >= 3)
					{
						allVertexData.Add(allVertexData[^(3 * i - 6)]);
						allVertexData.Add(allVertexData[^2]);
					}

					allVertexData.Add(vert);
				}
			}
		}

		Mesh mesh = new(allVertexData.Select(v => v.Position).ToArray(), allVertexData.Select(v => v.Normal).ToArray(), allVertexData.Select(v => v.TexCoord).ToArray());
		return mesh;
	}


	struct VertexData
	{
		public float3 Position;
		public float3 Normal;
		public float2 TexCoord;
	}

	static string[] SplitByLine(string text, bool removeEmptyEntries = true)
	{
		StringSplitOptions options = removeEmptyEntries ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None;
		return text.Split(newLineStrings, options);
	}
}
