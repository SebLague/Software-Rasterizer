namespace SoftwareRasterizer.Types;

	public class Camera
	{
		public float Fov = Maths.ToRadians(60);
		public Transform Transform = new();
	}