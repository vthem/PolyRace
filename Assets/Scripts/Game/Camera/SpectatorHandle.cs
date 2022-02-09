using TSW;

using UnityEngine;

namespace Game.Camera
{
	public class SpectatorHandle : RacerHandle
	{
		public float _raycastDistance = 1000f;
		public float _cameraGroundOffset;
		public float _forwardLookupStep;
		public float _forwardLookupLength;
		public float _rightLookupStep;
		public float _rightLookupLength;
		public LayerMask _layerMask;
		public float _minShotDuration = 1.5f;
		private float _lastCameraUpdateTime;
		private readonly TSW.Follow _follow;
		private TSW.Camera.LookAt _lookAt;
		private TSW.Camera.Zoom _zoom;

		/// <summary>
		/// Look for position camera position in front of the racer
		/// Iterates through possible position, from left to right, then forward. Validate a position, and keep
		/// farther position only
		/// </summary>
		/// <returns><c>true</c>, if spectator camera position was updated, <c>false</c> otherwise.</returns>
		/// <param name="lookAtTargetPosition">Look at target position.</param>
		/// <param name="lookAtTargetForward">Look at target forward.</param>
		private bool UpdateSpectatorCameraPosition(Vector3 lookAtTargetPosition, Vector3 lookAtTargetForward)
		{
			float maxDistance = 0.0f;
			float distance = 0.0f;
			bool found = false;
			Vector3 left = Vector3.Cross(lookAtTargetForward, Vector3.up).normalized;
			Vector3 start = lookAtTargetPosition + left * _rightLookupLength / 2f;
			start.y = _raycastDistance;
			_lastCameraUpdateTime = Time.time;
			Vector3 vTest = Vector3.zero;
			int error1, error2, error3, test;
			error1 = error2 = error3 = test = 0;

			foreach (Vector3 vx in VectorUtils.Range(start, -left, _rightLookupStep, _rightLookupLength))
			{
				foreach (Vector3 vz in VectorUtils.Range(vx, lookAtTargetForward, _forwardLookupStep, _forwardLookupLength))
				{
					test++;
					RaycastHit hit;
					if (Physics.Raycast(vz, Vector3.down, out hit, _raycastDistance, _layerMask))
					{
						vTest = vz;
						vTest.y = hit.point.y + _cameraGroundOffset;
						if (ValidateCameraPosition(lookAtTargetPosition, vTest, ref distance))
						{
							if (distance > maxDistance)
							{
								maxDistance = distance;
								transform.position = vTest;
								found = true;
							}
							else
							{
								error1++;
							}
						}
						else
						{
							error2++;
						}
					}
					else
					{
						error3++;
					}
				}
			}
			if (!found)
			{
				Debug.Log("Could not find valid position error1:" + error1 + " error2:" + error2 + " error3:" + error3 + " test:" + test);
			}
			return found;
		}

		private bool ValidateCameraPosition(Vector3 lookAtTarget, Vector3 position, ref float distance)
		{
			Vector3 vector = lookAtTarget - position;
			distance = vector.magnitude;
			if (distance > _zoom.MaxDistance)
			{
				return false;
			}
			if (distance < _zoom.MinDistance)
			{
				return false;
			}
			if (Physics.Raycast(position, vector, distance, _layerMask))
			{
				return false;
			}
			return true;
		}

		private void Update()
		{
			if (Controller != null)
			{
				if (Time.time > _lastCameraUpdateTime + _minShotDuration)
				{
					float distance = 0f;
					if (!ValidateCameraPosition(Controller.transform.position, transform.position, ref distance))
					{
						if (UpdateSpectatorCameraPosition(Controller.transform.position, Controller.transform.forward))
						{
							_lookAt.UpdateLookAt();
							_zoom.UpdateZoom();
						}
					}
				}
			}
		}

		protected override void HandleInitialize()
		{
			base.HandleInitialize();
			_lookAt = GetComponent<TSW.Camera.LookAt>();
			_zoom = GetComponent<TSW.Camera.Zoom>();
			_lookAt.Target = Controller.transform;
			_zoom.Target = Controller.transform;
			_zoom.ActiveCamera = ActiveCamera;
			if (UpdateSpectatorCameraPosition(Controller.transform.position, Controller.transform.forward))
			{
				_lookAt.UpdateLookAt();
				_zoom.UpdateZoom();
			}
			else
			{
				Debug.LogWarning("Could not enable camera");
			}
		}
	}
}
