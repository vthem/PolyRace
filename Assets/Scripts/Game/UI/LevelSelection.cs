using System.Collections;
using System.Text.RegularExpressions;

using TSW.Messaging;

using LevelGen;

using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	[RequireComponent(typeof(ScanForHandler))]
	public class LevelSelection : UIElement, INavigationOption
	{
		private bool _allowSeedSelection = false;

		[SerializeField]
		private GameObject _randomGroup;

		[SerializeField]
		private InputField _seedInput;

		[SerializeField]
		private Sprite[] _regionSprites;

		[SerializeField]
		private Image _previewImage;

		[SerializeField]
		private Text _seedFormatHelpText;

		public bool AllowSeedSelection { get => _allowSeedSelection; set => _allowSeedSelection = value; }

		public void OnRandomizeButton()
		{
			StartCoroutine(RandomizeLevel());
		}

		private IEnumerator RandomizeLevel()
		{
			LevelIdentifier levelId;
			levelId = LevelIdentifier.Randomize();
			Race.Setup.Next.UpdateLevelIdentifier(levelId);
			UpdateInput();
			yield break;
		}

		public void OnEndEditSeed(string str)
		{
			string s = _seedInput.text.ToLower();
			if (Regex.IsMatch(s, @"^[a-z]{2}[0-9]{2}[a-z]{2}$"))
			{
				ClearSeedFormatHelp();
				LevelIdentifier levelId = new LevelIdentifier(_seedInput.text);
				Race.Setup.Next.UpdateLevelIdentifier(levelId);
			}
			else
			{
				_seedInput.text = "az09az";
				SetSeedFormatHelp("SeedFormatHelp");
			}
		}

		private void SetSeedFormatHelp(string locKey)
		{
			_seedFormatHelpText.text = TSW.Loca.DirtyLoca.GetTextValue(locKey);
			_seedFormatHelpText.gameObject.SetActive(true);
		}

		private void ClearSeedFormatHelp()
		{
			_seedFormatHelpText.gameObject.SetActive(false);
		}

		[EventHandler(typeof(Race.Setup.UpdatedEvent))]
		public void OnRaceSetupUpdate(Race.Setup.UpdatedEvent evt)
		{
			UpdateInput();
		}

		protected override IEnumerator OnDisplay()
		{
			Race.Setup.Backup();
			ClearSeedFormatHelp();
			UpdateInput();
			yield return null;
		}

		private void UpdateInput()
		{
			_randomGroup.SetActive(_allowSeedSelection);
			_seedInput.text = Race.Setup.Next.LevelIdentifier.SeedString;
			_previewImage.sprite = _regionSprites[(int)Race.Setup.Next.LevelIdentifier.LevelRegion * 4 + (int)Race.Setup.Next.LevelIdentifier.LevelDifficulty];
		}

		private int NextEnum(int cur, System.Type enumType)
		{
			return Mathf.Abs((cur + 1) % System.Enum.GetNames(enumType).Length);
		}

		private int PreviousEnum(int cur, System.Type enumType)
		{
			if (--cur < 0)
			{
				cur = System.Enum.GetNames(enumType).Length - 1;
			}
			return cur;
		}

		private void SetLocalizedText(string localizationKey, Text textObject)
		{
			textObject.text = TSW.Loca.DirtyLoca.GetTextValue(localizationKey);
		}

		public string[] GetActiveButton()
		{
			return new string[] { "Cancel_Button", "Validate_Button" };
		}

		public string GetFocusButton()
		{
			return string.Empty;
		}
	}
}
