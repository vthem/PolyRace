using TSW;

using UnityEngine;

namespace Game.ParticleEffect
{
	[RequireComponent(typeof(VelocityMeter))]
	[RequireComponent(typeof(ParticleSystem))]
	public class SpeedControlParticleEmitRate : MonoBehaviour
	{
		private ParticleSystem _particleSystem;
		private VelocityMeter _velocityMeter;


		[SerializeField]
		private float _minEmit;

		[SerializeField]
		private float _maxEmit;

		[SerializeField]
		private float _minSpeed;

		[SerializeField]
		private float _maxSpeed;

		private void Start()
		{
			_particleSystem = GetComponent<ParticleSystem>();
			_velocityMeter = GetComponent<VelocityMeter>();
		}

		private void Update()
		{
			float normSpeed = (Mathf.Clamp(_velocityMeter.Velocity.z, _minSpeed, _maxSpeed) - _minSpeed) / (_maxSpeed - _minSpeed);
			_particleSystem.SetEmissionRate(Mathf.Lerp(_minEmit, _maxEmit, normSpeed));
		}
	}
}
