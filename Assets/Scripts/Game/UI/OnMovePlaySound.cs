using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.UI
{
	public class OnMovePlaySound : MonoBehaviour, IMoveHandler
	{
		[SerializeField]
		private string _soundName = "UIButtonMove";

		public void OnMove(AxisEventData eventData)
		{
			Audio.SoundFx.Instance.Play(_soundName);
		}
	}
}
