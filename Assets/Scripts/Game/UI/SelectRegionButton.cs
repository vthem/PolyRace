using System;

using TSW.Messaging;

using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	[RequireComponent(typeof(Button))]
	[RequireComponent(typeof(ScanForHandler))]
	public class SelectRegionButton : MonoBehaviour
	{
		[SerializeField]
		private LevelGen.LevelRegion _region;

		[SerializeField]
		private SelectedButton _selectedButton;

		private void Start()
		{
			GetComponent<Button>().onClick.AddListener(() =>
			{
				LevelGen.LevelDifficulty difficulty = Race.Setup.Next.LevelIdentifier.LevelDifficulty;
				Race.Setup.Next.UpdateLevelIdentifier(new LevelGen.LevelIdentifier(_region, difficulty));
			});
		}

		[EventHandler(typeof(Race.Setup.UpdatedEvent))]
		public void OnRaceSetupChanged(Race.Setup.UpdatedEvent evt)
		{
			UpdateSelectedButton(evt.Value1.LevelIdentifier.LevelRegion);
		}

		private void UpdateSelectedButton(LevelGen.LevelRegion region)
		{
			if (region == _region)
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
				UpdateSelectedButton(Race.Setup.Next.LevelIdentifier.LevelRegion);
			}
		}
	}
}