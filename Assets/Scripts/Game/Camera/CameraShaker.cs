using System.Collections.Generic;

using TSW.Messaging;

using DG.Tweening;

using UnityEngine;

using BDEvent = TSW.Messaging.Event;

namespace Game.Camera
{
	[RequireComponent(typeof(ScanForHandler))]
	public class CameraShaker : MonoBehaviour
	{
		[SerializeField]
		private float _strength = 90f;

		[SerializeField]
		private float _randomness = 90f;

		[SerializeField]
		private float _duration = 1f;

		[SerializeField]
		private int _vibrato = 10;

		protected List<Dispatcher.EventHandler> _handlers;

		[EventHandler(typeof(Racer.Modules.CollisionModule.CollisionEvent))]
		public void OnShieldActivated(BDEvent evt)
		{
			transform.DOComplete();
			transform.DOShakeRotation(_duration, _strength, _vibrato, _randomness);
		}

		[EventHandler(typeof(Racer.EnergyController.BatteryEmptyEvent))]
		public void OnBatteryEmpty(BDEvent evt)
		{
			transform.DOComplete();
			transform.DOShakeRotation(_duration, _strength, _vibrato, _randomness);
		}

		private void OnDestroy()
		{
			if (_handlers != null)
			{
				foreach (Dispatcher.EventHandler handler in _handlers)
				{
					Dispatcher.Instance.RemoveHandler(handler);
				}
			}
		}

		private void Start()
		{
			_handlers = TSW.Messaging.Dispatcher.AddHandler(this);
		}
	}
}
