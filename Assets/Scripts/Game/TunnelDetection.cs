using UnityEngine;
using UnityEngine.Events;

namespace Game
{

	public class TunnelDetection : MonoBehaviour
	{
		[SerializeField]
		private LayerMask _layer;

		[SerializeField]
		public UnityEvent _onEnterTunnel;

		[SerializeField]
		public UnityEvent _onExitTunnel;
		private int _inCount = 0;

		private void OnTriggerEnter(Collider other)
		{
			if (_inCount == 0)
			{
				//                Debug.Log("OnTunnelEnter count:" + _inCount);
				_onEnterTunnel.Invoke();
			}
			else
			{
				//                Debug.Log("TunnelDetection (enter) count:" + _inCount);
			}
			_inCount++;
		}

		private void OnTriggerExit(Collider other)
		{
			_inCount--;
			if (_inCount == 0)
			{
				//                Debug.Log("OnTunnelExit count:" + _inCount);
				_onExitTunnel.Invoke();
			}
			else
			{
				//                Debug.Log("TunnelDetection (exit) count:" + _inCount);
			}
		}
	}
}
