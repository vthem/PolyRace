using System.Collections;

namespace Game.UI
{
	public class Training : UIElement, INavigationOption
	{
		protected override IEnumerator OnBeforeDisplay()
		{
			if (Race.Setup.Next == null || Race.Setup.Next.GameMode != GameMode.Training)
			{
				Race.Setup.New();
				yield return StartCoroutine(Race.Setup.Next.RandomizeIfInvalid((levelId) => false));
			}
			Race.Setup.Next.UpdateMode(GameMode.Training);

			yield return StartCoroutine(Race.Setup.Next.RandomizeIfInvalid((levelId) => false));
		}

		protected override IEnumerator OnDisplay()
		{
			Race.Setup.Next.NotifyUpdate();
			yield return null;
		}

		public string[] GetActiveButton()
		{
			return new string[] { "MainMenu_Button" };
		}

		public string GetFocusButton()
		{
			return string.Empty;
		}
	}
}
