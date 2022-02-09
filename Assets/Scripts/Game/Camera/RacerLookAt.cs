using UnityEngine;

namespace Game.Camera
{
	public class RacerLookAt : TSW.Camera.LookAt
	{
		[SerializeField]
		private Game.Racer.Controller _racerController;

		[SerializeField]
		private float _targetRightOffset = 1f;

		public void SetRacerController(Game.Racer.Controller racerController)
		{
			_racerController = racerController;
			Target = _racerController.transform;
		}

		protected override Vector3 ComputeTargetNormal()
		{
			return ((_target.TransformPoint(_offset) + _target.right * _racerController.DataModule.NormalizedAngularSpeed * _targetRightOffset) - transform.position).normalized;
		}
	}
}