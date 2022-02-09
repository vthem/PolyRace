using UnityEngine;

namespace Game.UI
{
	public class HovercraftCycleButton : MonoBehaviour
	{
		public void OnPreviousRacerButton()
		{
			int currentId = Race.Setup.Next.RacerDynamicProperties.Properties.Id;
			currentId -= 1;

			if (currentId < 0)
			{
				currentId = Racer.PropertiesCollection.GetAsset().Count - 1;
			}
			Race.Setup.Next.UpdateRacerDynamicProperties(Racer.DynamicProperties.NewFromRacerId(currentId));
		}

		public void OnNextRacerButton()
		{
			int currentId = Race.Setup.Next.RacerDynamicProperties.Properties.Id;
			currentId += 1;
			if (currentId > Racer.PropertiesCollection.GetAsset().Count - 1)
			{
				currentId = 0;
			}
			Race.Setup.Next.UpdateRacerDynamicProperties(Racer.DynamicProperties.NewFromRacerId(currentId));
		}
	}
}
