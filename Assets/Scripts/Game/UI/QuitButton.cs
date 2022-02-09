using System.Collections;

using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class QuitButton : MonoBehaviour
	{
		[SerializeField]
		private Text _quitButtonText;

		[SerializeField]
		private float _confirmWaitTime = 1f;
		private bool _confirm = false;

		public void OnQuitButton()
		{
			if (!_confirm)
			{
				StartCoroutine(WaitConfirm());
			}
			else
			{
				Application.Quit();
			}
		}

		private IEnumerator WaitConfirm()
		{
			UpdateText("Confirm");
			_confirm = true;
			yield return new WaitForSeconds(_confirmWaitTime);
			_confirm = false;
			UpdateText("Quit");
		}

		private void UpdateText(string localizedKey)
		{
			_quitButtonText.text = TSW.Loca.DirtyLoca.GetTextValue(localizedKey);
		}
	}
}