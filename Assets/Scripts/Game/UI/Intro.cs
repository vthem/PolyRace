using System;
using System.Collections;
using System.Collections.Generic;

using DG.Tweening;

using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class Intro : UIElement
	{
		[SerializeField]
		private ActiveSwitcher _switcher;

		[SerializeField]
		private Text _pressAnyKeyText;

		[SerializeField]
		private float _punchForce = 1f;

		[SerializeField]
		private int _punchVibrato = 10;

		[SerializeField]
		private GameObject _demoText;
		private Text _punchText;
		private readonly List<Func<bool>> _bootSequence = new List<Func<bool>>();

		protected override IEnumerator OnDisplay()
		{
			InvokeRepeating("TextBeat", 0, 1f);
			_demoText.SetActive(Config.GetAsset().IsDemo);
			_bootSequence.Add(BootPlayer);
			BootSequence();
			yield return null;
		}

		private void OnPlayerDataInitialized()
		{
			Player.PlayerManager.Instance.OnInitialized -= OnPlayerDataInitialized;
			BootSequence();
		}

		private void BootSequence()
		{
			for (int i = 0; i < _bootSequence.Count; ++i)
			{
				if (!_bootSequence[i]())
				{
					Debug.Log("Boot sequence canceled at " + _bootSequence[i].Method.Name);
					return;
				}
			}
			PressAnyKeyMode();
		}

		private bool BootPlayer()
		{
			if (!Player.PlayerManager.Instance.Initialized)
			{
				Player.PlayerManager.Instance.OnInitialized += OnPlayerDataInitialized;
				Player.PlayerManager.Instance.Initialize();
				return false;
			}
			return true;
		}

		private void PressAnyKeyMode()
		{
			_pressAnyKeyText.gameObject.SetActive(true);
			_punchText = _pressAnyKeyText;
			Timer.Instance.Cancel("wait-server");
		}

		protected override IEnumerator OnHideComplete()
		{
			CancelInvoke("TextBeat");
			yield return null;
		}

		private void Update()
		{
			if (!DisplayState)
			{
				return;
			}
			if (!_pressAnyKeyText.gameObject.activeSelf)
			{
				return;
			}
			// TODO
			//if (UnityInput.GetMouseButtonDown(0) || UnityInput.anyKeyDown) {
			//	AnimateDisplayState(false,
			//		() => {
			//			gameObject.SetActive(false);
			//                     _switcher.Switch("MainMenu_UIElement");
			//		}
			//	);
			//}
			if (Game.Input.InputManager.PressAnyKey())
			{
				AnimateDisplayState(false,
					() =>
					{
						gameObject.SetActive(false);
						_switcher.Switch("MainMenu_UIElement");
					}
				);
			}
		}

		private void TextBeat()
		{
			_punchText.transform.DOPunchScale(Vector3.one * _punchForce, 0.5f, _punchVibrato);
		}
	}
}
