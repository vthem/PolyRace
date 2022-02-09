using TSW.Messaging;

using DG.Tweening;

using UnityEngine;

namespace Game.Camera
{
	[RequireComponent(typeof(UnityEngine.Camera))]
	[RequireComponent(typeof(ScanForHandler))]
	public class FovBoost : MonoBehaviour
	{
		[SerializeField]
		private float _boostOffset = 10;

		[SerializeField]
		private float _tweenDuration = .5f;
		private UnityEngine.Camera _camera;
		private float _baseFov;
		private Tweener _fovTweener;

		private void Start()
		{
			_camera = GetComponent<UnityEngine.Camera>();
			_baseFov = _camera.fieldOfView;
		}


		[EventHandler(typeof(Racer.EnergyController.BoostActivatedEvent))]
		public void OnBoostActivated(Racer.EnergyController.BoostActivatedEvent evt)
		{
			if (_fovTweener != null)
			{
				_fovTweener.Kill();
			}
			_fovTweener = _camera.DOFieldOfView(_baseFov + _boostOffset, _tweenDuration);
		}

		[EventHandler(typeof(Racer.EnergyController.BoostDeactivatedEvent))]
		public void OnBoostDeactivated(Racer.EnergyController.BoostDeactivatedEvent evt)
		{
			if (_fovTweener != null)
			{
				_fovTweener.Kill();
			}
			_fovTweener = _camera.DOFieldOfView(_baseFov, _tweenDuration);
		}
	}
}