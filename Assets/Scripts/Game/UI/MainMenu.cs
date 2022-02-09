using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class MainMenu : UIElement, INavigationOption
	{
		[SerializeField]
		private Selectable _demoFocus;

		public string[] GetActiveButton()
		{
			if (Config.GetAsset().IsDemo)
			{
				return new string[] { "Options_Button", "Quit_Button" };
			}
			else
			{
				return new string[] { "Dashboard_Layout", "Options_Button", "Quit_Button" };
			}
		}

		public override void SetFocus()
		{
			if (Config.GetAsset().IsDemo)
			{
				SetFocus(_demoFocus);
			}
			else
			{
				base.SetFocus();
			}
		}

		public string GetFocusButton()
		{
			return string.Empty;
		}
	}
}
