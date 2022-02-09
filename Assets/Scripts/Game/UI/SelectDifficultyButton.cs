using System;

using TSW.Messaging;

using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	[RequireComponent(typeof(Button))]
	[RequireComponent(typeof(ScanForHandler))]
	public class SelectDifficultyButton : MonoBehaviour
	{
		[SerializeField]
		private LevelGen.LevelDifficulty _difficulty;

		[SerializeField]
		private SelectedButton _selectedButton;

		private void Start()
		{
			GetComponent<Button>().onClick.AddListener(() =>
			{
				LevelGen.LevelRegion region = Race.Setup.Next.LevelIdentifier.LevelRegion;
				Race.Setup.Next.UpdateLevelIdentifier(new LevelGen.LevelIdentifier(region, _difficulty));
			});
		}

		[EventHandler(typeof(Race.Setup.UpdatedEvent))]
		public void OnRaceSetupChanged(Race.Setup.UpdatedEvent evt)
		{
			UpdateSelectedButton(evt.Value1.LevelIdentifier.LevelDifficulty);
		}

		private void UpdateSelectedButton(LevelGen.LevelDifficulty difficulty)
		{
			if (difficulty == _difficulty)
			{
				_selectedButton.SetAsSelected();
			}
			else
			{
				_selectedButton.SetAsNotSelected();
			}
		}

		private void OnEnable()
		{
			if (Race.Setup.Next != null)
			{
				UpdateSelectedButton(Race.Setup.Next.LevelIdentifier.LevelDifficulty);
			}
		}
	}
}