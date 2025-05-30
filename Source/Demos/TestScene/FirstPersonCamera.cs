using SoftwareRasterizer.Types;
using SoftwareRasterizer.Core;
using static SoftwareRasterizer.Maths;

namespace SoftwareRasterizer.Demo;

public class FirstPersonCamera(Transform transform)
{
	float camYawTarget;
	float camPitchTarget;

	public void Update(float deltaTime, float2 screenSize)
	{
		// Rotate camera with mouse
		const float mouseSensitivity = 2f;
		if (Input.IsHoldingMouse(MouseButton.Left))
		{
			float2 mouseDelta = Input.GetMouseDelta() / screenSize.x * mouseSensitivity;
			camPitchTarget = Clamp(camPitchTarget - mouseDelta.y, ToRadians(-85), ToRadians(85));
			camYawTarget -= mouseDelta.x;
			Input.LockCursor();
		}
		else if (Input.IsKeyDownThisFrame(Key.Q))
		{
			Input.UnlockCursor();
			
		}

		transform.Pitch = Lerp(transform.Pitch, camPitchTarget, deltaTime * 15);
		transform.Yaw = Lerp(transform.Yaw, camYawTarget, deltaTime * 15);

		// Move camera with WASD
		const float camSpeed = 2.5f;
		float3 moveDelta = float3.Zero;

		if (Input.IsKeyHeld(Key.W)) moveDelta += transform.Forward;
		if (Input.IsKeyHeld(Key.S)) moveDelta -= transform.Forward;
		if (Input.IsKeyHeld(Key.A)) moveDelta -= transform.Right;
		if (Input.IsKeyHeld(Key.D)) moveDelta += transform.Right;

		transform.Position += float3.Normalize(moveDelta) * camSpeed * deltaTime;
		transform.Position.y = 1;
	}
	
}