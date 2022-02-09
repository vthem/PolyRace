using UnityEngine;

namespace TSW.Algorithm
{
	[System.Serializable]
	public class PIDController
	{
		[SerializeField]
		private float _pGain = .5f;


		[SerializeField]
		private float _iGain = .5f;

		[SerializeField]
		private float _dGain = .5f;
		private float _integrator;
		private float _lastError;

		public float PGain
		{
			get => _pGain;

			set => _pGain = value;
		}

		public float IGain
		{
			get => _iGain;

			set => _iGain = value;
		}

		public float DGain
		{
			get => _dGain;

			set => _dGain = value;
		}

		public PIDController(float pGain, float iGain, float dGain)
		{
			_pGain = pGain;
			_iGain = iGain;
			_dGain = dGain;
		}

		public float Update(float target, float value, float time)
		{
			float error = target - value;
			_integrator += error * time;
			float diff = (error - _lastError) / time;
			_lastError = error;
			return error * _pGain + _integrator * _iGain + diff * _dGain;
		}
	}
}
