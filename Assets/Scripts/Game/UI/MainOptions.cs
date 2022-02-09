using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class MainOptions : UIElement, INavigationOption
	{
		[SerializeField]
		private Text _resolutionValueText;

		[SerializeField]
		private Text _fullscreenValueText;

		[SerializeField]
		private Text _qualityValueText;

		[SerializeField]
		private ValidateVideoSettings _validateVideoSettingsWindow;

		[SerializeField]
		private Text _speedUnitText;

		private class VideoSettings
		{
			public Resolution Resolution { get; private set; }
			public bool FullScreen { get; private set; }
			public int Quality { get; private set; }

			public VideoSettings()
			{
				Resolution = Screen.currentResolution;
				FullScreen = Screen.fullScreen;
				Quality = QualitySettings.GetQualityLevel();
			}

			public void ChangeResolution(int offset)
			{
				int curIndex = -1;
				for (int i = 0; i < Screen.resolutions.Length; ++i)
				{
					if (Resolution.height == Screen.resolutions[i].height && Resolution.width == Screen.resolutions[i].width)
					{
						curIndex = i;
						break;
					}
				}
				if (curIndex == -1)
				{
					Debug.Log("not found");
					return;
				}
				curIndex += offset;
				if (curIndex >= Screen.resolutions.Length)
				{
					curIndex = 0;
				}
				if (curIndex < 0)
				{
					curIndex = Screen.resolutions.Length - 1;
				}
				Resolution = Screen.resolutions[curIndex];
			}

			public void ChangeFullScreen()
			{
				FullScreen = !FullScreen;
			}

			public void ChangeQuality(int offset)
			{
				Quality += offset;
				if (Quality >= QualitySettings.names.Length)
				{
					Quality = 0;
				}
				if (Quality < 0)
				{
					Quality = QualitySettings.names.Length - 1;
				}
			}

			public void Apply()
			{
				QualitySettings.SetQualityLevel(Quality);
				Screen.SetResolution(Resolution.width, Resolution.height, FullScreen);
			}
		}

		private VideoSettings _newVideoSettings;
		private VideoSettings _backupVideoSettings;

		private void ChangeLanguage(int offset)
		{
			List<string> langs = new List<string>(TSW.Loca.DirtyLoca.AvailableLanguageCode);
			string current = TSW.Loca.DirtyLoca.CurrentLanguageCode;
			int index = langs.IndexOf(current);
			index += offset;
			if (index >= langs.Count)
			{
				index = 0;
			}
			if (index < 0)
			{
				index = langs.Count - 1;
			}
			TSW.Loca.DirtyLoca.UseLanguage(langs[index]);
			Player.PlayerManager.Instance.SetLanguage(langs[index]);
		}

		protected override IEnumerator OnDisplay()
		{
			_newVideoSettings = new VideoSettings();
			UpdateValueText();
			yield return null;
		}

		public void OnNextResolutionButton()
		{
			_newVideoSettings.ChangeResolution(1);
			UpdateValueText();
		}

		public void OnPreviousResolutionButton()
		{
			_newVideoSettings.ChangeResolution(-1);
			UpdateValueText();
		}

		public void OnToggleFullScreenButton()
		{
			_newVideoSettings.ChangeFullScreen();
			UpdateValueText();
		}

		public void OnNextQualityButton()
		{
			_newVideoSettings.ChangeQuality(1);
			UpdateValueText();
		}

		public void OnPreviousQualityButton()
		{
			_newVideoSettings.ChangeQuality(-1);
			UpdateValueText();
		}

		public void OnNextLanguage()
		{
			ChangeLanguage(1);
			UpdateValueText();
		}

		public void OnPreviousLanguage()
		{
			ChangeLanguage(-1);
			UpdateValueText();
		}

		public void OnNextUnits()
		{
			SpeedSystemType unit = Player.PlayerManager.Instance.GetSpeedUnit();
			if (unit == SpeedSystemType.Imperial)
			{
				Player.PlayerManager.Instance.SetSpeedUnit(SpeedSystemType.International);
			}
			else
			{
				Player.PlayerManager.Instance.SetSpeedUnit(SpeedSystemType.Imperial);
			}
			UpdateValueText();
		}

		public void OnPreviousUnits()
		{
			OnNextUnits();
		}

		public void ApplyVideoSettings()
		{
			_newVideoSettings.Apply();
			_backupVideoSettings = new VideoSettings();
			_validateVideoSettingsWindow.OnCancelCallback = () =>
			{
				_backupVideoSettings.Apply();
				_newVideoSettings = _backupVideoSettings;
				_backupVideoSettings = null;
				StartCoroutine(_validateVideoSettingsWindow.AnimateDisplayState(false));
			};
			_validateVideoSettingsWindow.OnValidateCallback = () =>
			{
				StartCoroutine(_validateVideoSettingsWindow.AnimateDisplayState(false));
			};
			StartCoroutine(_validateVideoSettingsWindow.AnimateDisplayState(true));
		}

		private void UpdateValueText()
		{
			_resolutionValueText.text = _newVideoSettings.Resolution.width + "x" + _newVideoSettings.Resolution.height;
			SetLocalizedText(_newVideoSettings.FullScreen ? "Yes" : "No", _fullscreenValueText);
			SetLocalizedText(QualitySettings.names[_newVideoSettings.Quality], _qualityValueText);
			_speedUnitText.text = SpeedUnit.UnitString(Player.PlayerManager.Instance.GetSpeedUnit());
		}

		private void SetLocalizedText(string localizationKey, Text textObject)
		{
			string loc = TSW.Loca.DirtyLoca.GetTextValue(localizationKey);
			if (string.IsNullOrEmpty(loc))
			{
				Debug.LogWarning("could not find localization key:" + localizationKey + " object:" + name);
				return;
			}
			textObject.text = loc;
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
