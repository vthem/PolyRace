namespace Game.UI
{
	public class Credits : UIElement, INavigationOption
	{
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