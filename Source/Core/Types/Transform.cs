namespace SoftwareRasterizer.Types;

using static MathF;

public class Transform
{
	public float3 Position;
	public float3 Scale = float3.One;
	public Transform Parent;

	public float Pitch
	{
		get => _pitch;
		set => SetRotation(value, _yaw, _roll);
	}

	public float Yaw
	{
		get => _yaw;
		set => SetRotation(_pitch, value, _roll);
	}

	public float Roll
	{
		get => _roll;
		set => SetRotation(_pitch, _yaw, value);
	}

	public float3 Right => ihat;
	public float3 Up => jhat;
	public float3 Forward => khat;

	float _pitch;
	float _yaw;
	float _roll;

	float3 ihat;
	float3 jhat;
	float3 khat;
	float3 ihat_inv;
	float3 jhat_inv;
	float3 khat_inv;

	public Transform()
	{
		UpdateBasisVectors();
	}

	public void SetPosRotScale(float3 pos, float3 angles, float3 scale)
	{
		Position = pos;
		Scale = scale;
		SetRotation(angles.x, angles.y, angles.z);
	}

	public void SetRotation(float pitch, float yaw, float roll)
	{
		_pitch = pitch;
		_yaw = yaw;
		_roll = roll;
		UpdateBasisVectors();
	}


	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public float3 ToWorldPoint(float3 localPoint)
	{
		float3 p = localPoint;
		p = TransformVector(ihat * Scale.x, jhat * Scale.y, khat * Scale.z, p) + Position;
		if (Parent != null) p = Parent.ToWorldPoint(p);
		return p;
	}

	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public float3 ToLocalPoint(float3 worldPoint)
	{
		float3 p = worldPoint;
		if (Parent != null) p = Parent.ToLocalPoint(p);
		p = TransformVector(ihat_inv, jhat_inv, khat_inv, p - Position);
		p.x /= Scale.x;
		p.y /= Scale.y;
		p.z /= Scale.z;
		return p;
	}
	
	void UpdateBasisVectors()
	{
		(ihat, jhat, khat) = GetBasisVectors();
		(ihat_inv, jhat_inv, khat_inv) = GetInverseBasisVectors();
	}
	
	// Calculate right/up/forward vectors (î, ĵ, k̂)
	(float3 ihat, float3 jhat, float3 khat) GetBasisVectors()
	{
		// ---- Yaw ----
		float3 ihat_yaw = new(Cos(Yaw), 0, Sin(Yaw));
		float3 jhat_yaw = new(0, 1, 0);
		float3 khat_yaw = new(-Sin(Yaw), 0, Cos(Yaw));
		// ---- Pitch ----
		float3 ihat_pitch = new(1, 0, 0);
		float3 jhat_pitch = new(0, Cos(Pitch), -Sin(Pitch));
		float3 khat_pitch = new(0, Sin(Pitch), Cos(Pitch));
		// ---- Roll ----
		float3 ihat_roll = new(Cos(Roll), Sin(Roll), 0);
		float3 jhat_roll = new(-Sin(Roll), Cos(Roll), 0);
		float3 khat_roll = new(0, 0, 1);
		// ---- Yaw and Pitch combined ----
		float3 ihat_pitchYaw = TransformVector(ihat_yaw, jhat_yaw, khat_yaw, ihat_pitch);
		float3 jhat_pitchYaw = TransformVector(ihat_yaw, jhat_yaw, khat_yaw, jhat_pitch);
		float3 khat_pitchYaw = TransformVector(ihat_yaw, jhat_yaw, khat_yaw, khat_pitch);
		// Combine roll
		float3 ihat = TransformVector(ihat_pitchYaw, jhat_pitchYaw, khat_pitchYaw, ihat_roll);
		float3 jhat = TransformVector(ihat_pitchYaw, jhat_pitchYaw, khat_pitchYaw, jhat_roll);
		float3 khat = TransformVector(ihat_pitchYaw, jhat_pitchYaw, khat_pitchYaw, khat_roll);
		return (ihat, jhat, khat);
	}

	(float3 ihat, float3 jhat, float3 khat) GetInverseBasisVectors()
	{
		(float3 ihat, float3 jhat, float3 khat) = GetBasisVectors();
		float3 ihat_inverse = new(ihat.x, jhat.x, khat.x);
		float3 jhat_inverse = new(ihat.y, jhat.y, khat.y);
		float3 khat_inverse = new(ihat.z, jhat.z, khat.z);
		return (ihat_inverse, jhat_inverse, khat_inverse);
	}


	// Move each coordinate of given vector along the corresponding basis vector
	[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	static float3 TransformVector(float3 ihat, float3 jhat, float3 khat, float3 v)
	{
		return v.x * ihat + v.y * jhat + v.z * khat;
	}
}