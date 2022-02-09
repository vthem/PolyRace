using UnityEngine;

namespace Game
{
	public class HoverAnimation : MonoBehaviour
	{
		public float _speed = 1f;
		public float _amplitudeRange = 1f;
		private float _amplitude = 1f;
		private float _initY;
		private float _targetAmplitude;
		private float _phase = 10f;

		// Use this for initialization
		private void Start()
		{
			_initY = transform.localPosition.y;
			UpdateAmplitude();
			_phase = Random.Range(0, 200f);
		}

		// Update is called once per frame
		private void Update()
		{
			_amplitude = Mathf.Lerp(_amplitude, _targetAmplitude, _speed * Time.deltaTime);
			transform.localPosition = new Vector3(
				transform.localPosition.x,
				_initY + _amplitude * Mathf.Sin(Time.time * _speed + _phase),
				transform.localPosition.z
			);
			if (Mathf.Abs(_amplitude - _targetAmplitude) < 0.1f)
			{
				UpdateAmplitude();
			}
		}

		private void UpdateAmplitude()
		{
			_targetAmplitude = Random.Range(_amplitudeRange / 2f, _amplitudeRange);
		}
	}
}
