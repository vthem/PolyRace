using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class ValidateVideoSettings : UIElement
	{
		[SerializeField]
		private Text _countDownText;

		public Action OnCancelCallback { get; set; }
		public Action OnValidateCallback { get; set; }

		protected override IEnumerator OnDisplay()
		{
			Timer.Instance.Arm("ValidateVideoSettings", true, 10f, OnCancelCallback);
			yield return null;
		}

		protected override IEnumerator OnHide()
		{
			Timer.Instance.Cancel("ValidateVideoSettings");
			yield return null;
		}

		public void OnValidateButton()
		{
			Timer.Instance.Cancel("ValidateVideoSettings");
			OnValidateCallback();
		}

		private void Update()
		{
			_countDownText.text = Mathf.RoundToInt(Timer.Instance.CurrentTime("ValidateVideoSettings")).ToString();
		}
	}
}