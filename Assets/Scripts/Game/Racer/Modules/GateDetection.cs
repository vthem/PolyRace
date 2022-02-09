using TSW.Messaging;

using UnityEngine;

using BDEvent = TSW.Messaging.Event;

namespace Game.Racer.Modules
{
	public class GateDetection : BaseModule
	{
		public class StartLinePassedEvent : BDEvent { }
		public class EndLinePassedEvent : BDEvent { }

		private int _gateLayerMask;
		private Vector3 _lastPosition;

		public override void ModuleUpdate()
		{
			RaycastHit hitInfo;
			Vector3 direction = (_lastPosition - Controller.transform.position).normalized;
			float length = (_lastPosition - Controller.transform.position).magnitude;
			if (length > 0f && Physics.Raycast(Controller.transform.position, direction, out hitInfo, length, _gateLayerMask))
			{
				if (hitInfo.transform.gameObject.tag == "FinishLine")
				{
					Dispatcher.FireEvent(new EndLinePassedEvent());
				}
				else
				{
					Dispatcher.FireEvent(new StartLinePassedEvent());
				}
			}
			_lastPosition = Controller.transform.position;
		}

		public override void Enable()
		{
			base.Enable();
			_gateLayerMask = 1 << LayerMask.NameToLayer("GateTrigger");
			_lastPosition = transform.position;
		}
	}
}
