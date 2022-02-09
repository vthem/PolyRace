using UnityEngine;

using UnityInput = UnityEngine.Input;

namespace Game.Camera
{
	public class FollowOrbitHandle : RacerHandle
	{
		[SerializeField]
		private float _angularSpeed = 60f;

		[SerializeField]
		private float _distance = 10f;

		[SerializeField]
		private float _zSpeed = 5f;

		[SerializeField]
		private float _yOffset = 10f;
		private float _angle;
		private TSW.Follow _follow;
		private RacerLookAt _lookAt;

		protected override void HandleInitialize()
		{
			base.HandleInitialize();
			_follow = GetComponent<TSW.Follow>();
			_follow.Target = Controller.transform;
			_follow.UpdatePosition();

			_lookAt = GetComponent<RacerLookAt>();
			_lookAt.SetRacerController(Controller);
			_lookAt.UpdateLookAt();
			UpdatePosition();
		}

		private void Update()
		{
			if (UnityInput.GetKey(KeyCode.L))
			{
				_angle += UnityInput.GetAxis("Mouse X") * _angularSpeed * Time.deltaTime;
				if (UnityInput.GetKey(KeyCode.J))
				{
					_yOffset -= _zSpeed * Time.deltaTime;
				}
				else if (UnityInput.GetKey(KeyCode.K))
				{
					_yOffset += _zSpeed * Time.deltaTime;
				}
				_distance -= UnityInput.GetAxis("Mouse ScrollWheel") * _zSpeed;
				UpdatePosition();
			}
		}

		private void UpdatePosition()
		{
			_follow.RelativePosition = new Vector3(Mathf.Cos(_angle) * _distance, _yOffset, Mathf.Sin(_angle) * _distance);
		}
	}
}
