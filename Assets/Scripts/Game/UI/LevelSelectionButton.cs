using UnityEngine;

namespace Game.UI
{
	public class LevelSelectionButton : SwitcherButton
	{
		[SerializeField]
		private bool _allowSeedSelection;

		protected override void OnButtonPressed()
		{
			(_UIElement as LevelSelection).AllowSeedSelection = _allowSeedSelection;
			base.OnButtonPressed();
		}
	}
}
