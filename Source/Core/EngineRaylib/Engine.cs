using Raylib_cs;
using System.Numerics;
using SoftwareRasterizer.Types;

namespace SoftwareRasterizer.Core;

public static class Engine
{

	public static void Run(RenderTarget target, Scene scene)
	{
		// Create window and prepare texture for display
		Raylib.SetTraceLogLevel(TraceLogLevel.Error);
		Raylib.InitWindow(target.Width * 1, target.Height * 1, "Coding Adventure: Software Rasterizer!");
		
		Texture2D texture = Raylib.LoadTextureFromImage(Raylib.GenImageColor(target.Width, target.Height, Color.Black));
		Raylib.SetTextureFilter(texture, TextureFilter.Point);
		Color[] texColBuffer = new Color [target.Width * target.Height * 4]; // RGBA

		// Render loop
		while (!Raylib.WindowShouldClose())
		{
			// Update and rasterize scene
			scene.Update(target, Raylib.GetFrameTime());
			Rasterizer.Render(target, scene.Data);

			// Write rasterizer output to texture and display on window
			Raylib.UpdateTexture(texture, texColBuffer);
			ToFlatByteArray(target, texColBuffer); // Interop

			Rectangle src = new(0, texture.Height, texture.Width, -texture.Height);
			Rectangle dest = new(0, 0, Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
			Vector2 origin = new(0, 0);

			Raylib.BeginDrawing();
			Raylib.DrawTexturePro(texture, src, dest, origin, 0.0f, Color.White);
			Raylib.DrawFPS(10, 10);
			Raylib.EndDrawing();
		}

		Raylib.CloseWindow();
	}

	static void ToFlatByteArray(RenderTarget renderTarget, Color[] data)
	{
		for (int i = 0; i < renderTarget.ColourBuffer.Length; i++)
		{
			float3 col = renderTarget.ColourBuffer[i];
			data[i] = new Color((int)(Math.Clamp(col.r, 0, 1) * 255), (int)(Math.Clamp(col.g, 0, 1) * 255), (int)(Math.Clamp(col.b, 0, 1) * 255), 255);
		}
		
	}
}