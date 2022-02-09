using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
	public class OnPressPlaySound : MonoBehaviour, IPointerClickHandler, ISubmitHandler, ISubmitSecondaryHandler
	{
		[SerializeField]
		private string _soundName = "UIButton";

		public void OnPointerClick(PointerEventData eventData)
		{
			PlaySound();
		}

		public void OnSecondarySubmit(BaseEventData eventData)
		{
			PlaySound();
		}

		public void OnSubmit(BaseEventData eventData)
		{
			PlaySound();
		}

		private void PlaySound()
		{
			if (GetComponent<Selectable>().IsInteractable())
			{
				Audio.SoundFx.Instance.Play(_soundName);
			}
		}
	}
}
