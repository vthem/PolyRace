using UnityEngine;

// set of helper functions to ease migration from unity 5.x
public static class AnimationHelper
{
	public static void SetStartSpeed(this ParticleSystem ps, float value)
	{
		ParticleSystem.MainModule main = ps.main;
		ParticleSystem.MinMaxCurve startSpeed = main.startSpeed;
		startSpeed.constant = value;
		main.startSpeed = startSpeed;
	}

	public static float GetStartSpeed(this ParticleSystem ps)
	{
		return ps.main.startSpeed.constant;
	}

	public static void SetEmissionRate(this ParticleSystem ps, float value)
	{
		ParticleSystem.EmissionModule mod = ps.emission;
		ParticleSystem.MinMaxCurve rot = mod.rateOverTime;
		rot.constant = value;
		mod.rateOverTime = rot;
	}

	public static float GetEmissionRate(this ParticleSystem ps)
	{
		return ps.emission.rateOverTime.constant;
	}

	public static void SetStartSize(this ParticleSystem ps, float value)
	{
		ParticleSystem.MainModule main = ps.main;
		ParticleSystem.MinMaxCurve startSize = main.startSize;
		startSize.constant = value;
		main.startSize = startSize;
	}

	public static float GetStartSize(this ParticleSystem ps)
	{
		return ps.main.startSize.constant;
	}

	public static void SetStartLifetime(this ParticleSystem ps, float value)
	{
		ParticleSystem.MainModule main = ps.main;
		ParticleSystem.MinMaxCurve v = main.startLifetime;
		v.constant = value;
		main.startLifetime = v;
	}

	public static float GetStartLifetime(this ParticleSystem ps)
	{
		return ps.main.startLifetime.constant;
	}
}
