using SoftwareRasterizer.Types;
using SoftwareRasterizer.Core;

namespace SoftwareRasterizer.Demo;

static class Program
{
	
	static void Main()
	{
		Scene scene = new FlyingScene();
		//scene = new TestScene();
		
		RenderTarget renderTarget = new(960, 540);
		Engine.Run(renderTarget, scene);
	}
	
}