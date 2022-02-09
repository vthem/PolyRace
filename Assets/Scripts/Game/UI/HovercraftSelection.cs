using System.Collections;

namespace Game.UI
{
	public class HovercraftSelection : UIElement, INavigationOption
	{
		protected override IEnumerator OnDisplay()
		{
			Race.Setup.Backup();
			yield return null;
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