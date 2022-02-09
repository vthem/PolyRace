using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.UI
{
	public class PodiumFreeView : UIBehaviour, IPointerDownHandler, IPointerExitHandler, IPointerUpHandler
	{
		[SerializeField]
		private TSW.Camera.MouseOrbit _orbitCamera;

		public void OnPointerDown(PointerEventData eventData)
		{
			_orbitCamera.enabled = true;
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			//			_orbitCamera.enabled = false;
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			_orbitCamera.enabled = false;
		}
	}
}
