using SoftwareRasterizer.Core;
using SoftwareRasterizer.Types;

namespace SoftwareRasterizer.Demo;

public class AirplaneController(Model model)
{
	public Model model = model;
	
	const float speed = 10;
	const float rollSpeed = MathF.PI / 2;
	const float pitchSpeed = MathF.PI / 3.5f;
	
	float3 position = new (0, 10, 0);
	float pitch;
	float yaw;
	float roll;
	float targetRoll;
	float targetYaw;
	float targetPitch;

	public void Update(float deltaTime)
	{
		Transform transform = model.Transform;
		float3 forward = transform.Forward;
		
		float deltaRoll = 0;
		float deltaPitch = 0;

		// ---- Keyboard ----
		if (Input.IsKeyHeld(Key.A)) deltaRoll += deltaTime * rollSpeed;
		if (Input.IsKeyHeld(Key.D)) deltaRoll -= deltaTime * rollSpeed;
		if (deltaRoll == 0) targetRoll = 0;

		if (Input.IsKeyHeld(Key.W)) deltaPitch -= deltaTime * pitchSpeed;
		if (Input.IsKeyHeld(Key.S)) deltaPitch += deltaTime * pitchSpeed;
		if (deltaPitch == 0) targetPitch = 0;

		if (Input.IsKeyHeld(Key.LeftShift))
		{
			deltaRoll *= 0.4f;
			deltaPitch *= 0.4f;
		}

		position += forward * deltaTime * speed * 1;

		// Update targets
		targetRoll += deltaRoll;
		targetPitch += deltaPitch;
		targetYaw += deltaRoll * 0.5f;

		// Clamp
		float pitchMax = 30 * Maths.DegreesToRadians;
		float rollMax = 42 * Maths.DegreesToRadians;

		targetPitch = Math.Clamp(targetPitch, -pitchMax, pitchMax);
		targetRoll = Math.Clamp(targetRoll, -rollMax, rollMax);

		// Smooth
		roll = Maths.Lerp(roll, targetRoll, deltaTime * 2);
		yaw = Maths.Lerp(yaw, targetYaw, deltaTime * 3);
		pitch = Maths.Lerp(pitch, targetPitch, deltaTime * 3);

		transform.SetRotation(pitch, yaw, roll);
		transform.Position = position;
	}
}