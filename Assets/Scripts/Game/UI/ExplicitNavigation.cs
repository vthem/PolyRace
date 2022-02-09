using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
	public class ExplicitNavigation : UIBehaviour, IMoveHandler
	{
		[SerializeField]
		private Selectable _target;

		[SerializeField]
		private MoveDirection _moveDirection;

		public void OnMove(AxisEventData eventData)
		{
			if (eventData.moveDir == _moveDirection)
			{
				EventSystem.current.SetSelectedGameObject(_target.gameObject);
			}
		}
	}
}