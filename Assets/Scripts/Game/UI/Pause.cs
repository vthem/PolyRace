namespace Game.UI
{
	public class Pause : UIElement, INavigationOption
	{
		public string[] GetActiveButton()
		{
			return new string[] { "Resume_Button", "Restart_Button", "Options_Button", "", "MainMenu_Button" };
		}

		public string GetFocusButton()
		{
			return "Restart_Button";
		}
	}
}
