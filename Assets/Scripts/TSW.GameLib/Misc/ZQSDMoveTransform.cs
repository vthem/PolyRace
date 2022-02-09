using UnityEngine;

namespace TSW
{
	public class ZQSDMoveTransform : MonoBehaviour
	{
		public float _speedStep = 1f;
		public float _speed = 1f;

		// Update is called once per frame
		private void Update()
		{
			float mWheel = Input.GetAxis("Mouse ScrollWheel");
			if (mWheel > 0f)
			{
				_speed += _speedStep;
			}
			else if (mWheel < 0f)
			{
				_speed -= _speedStep;
			}
			if (Input.GetKey(KeyCode.Z))
			{
				transform.Translate(transform.forward * Time.deltaTime * _speed, Space.World);
			}
			if (Input.GetKey(KeyCode.S))
			{
				transform.Translate(-transform.forward * Time.deltaTime * _speed, Space.World);
			}
			if (Input.GetKey(KeyCode.D))
			{
				transform.Translate(transform.right * Time.deltaTime * _speed, Space.World);
			}
			if (Input.GetKey(KeyCode.Q))
			{
				transform.Translate(-transform.right * Time.deltaTime * _speed, Space.World);
			}

		}
	}
}
