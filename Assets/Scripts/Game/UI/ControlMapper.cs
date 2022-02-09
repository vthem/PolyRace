using System.Collections;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
	public class ControlMapper : UIElement, ISelectHandler, INavigationOption
	{
		//[SerializeField]
		//Rewired.UI.ControlMapper.ControlMapper _rewiredControlMapper;

		[SerializeField]
		private Selectable _cancelSelect;

		[SerializeField]
		private Text _gamepadDescriptionText;

		//void CancelCallback(InputActionEventData eventData) {
		//	EventSystem.current.SetSelectedGameObject(gameObject);
		//	ReInput.players.GetPlayer(0).RemoveInputEventDelegate(CancelCallback);
		//}

		public override void Initialize()
		{
			base.Initialize();
			//PlayerActivity.Instance.OnJoystickChanged += (playerActivity) => {
			//    if (DisplayState) {
			//        //ReloadPanel(playerActivity.ActiveJoystick);
			//    }
			//};
		}

		protected override IEnumerator OnDisplay()
		{
			//PlayerActivity.Instance.enabled = false;
			//ReloadPanel(PlayerActivity.Instance.ActiveJoystick);
			yield return null;
		}

		protected override IEnumerator OnHide()
		{
			//         PlayerActivity.Instance.enabled = true;
			//         ReInput.userDataStore.Save();
			//_rewiredControlMapper.Close(true);
			yield return null;
		}

		//void ReloadPanel(Rewired.Controller joystickController) {
		//    //_rewiredControlMapper.Open();
		//    //if (null == joystickController) {
		//    //    _gamepadDescriptionText.text = "--";
		//    //} else {
		//    //    _gamepadDescriptionText.text = joystickController.name;
		//    //}

		//}


		public void OnSelect(BaseEventData eventData)
		{
			EventSystem.current.SetSelectedGameObject(_cancelSelect.gameObject);
			eventData.Use();
		}

		public string[] GetActiveButton()
		{
			return new string[] { "AudioVideoLocalization_Button", "Control_Button", "About_Button", "ExitOptions_Button" };
		}

		public string GetFocusButton()
		{
			return string.Empty;
		}
	}
}
