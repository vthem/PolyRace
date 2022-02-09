namespace Game.UI
{
	public class Replay : UIElement, INavigationOption
	{
		public string[] GetActiveButton()
		{
			return new string[] { "ExitReplay_Button" };
		}

		public string GetFocusButton()
		{
			return string.Empty;
		}
	}
}
