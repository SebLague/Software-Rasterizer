using SoftwareRasterizer.Types;

namespace SoftwareRasterizer;

public abstract class Scene
{
	public SceneData Data = new();
	
	public abstract void Update(RenderTarget target, float deltaTime);
}