using TSW.Messaging;

using UnityEngine;

namespace Game.UI
{
	[RequireComponent(typeof(ScanForHandler))]
	public class GarageCharacteristicLine : MonoBehaviour
	{
		[SerializeField]
		private UnityEngine.UI.Text _label;

		[SerializeField]
		private UnityEngine.UI.Text _value;

		private void OnEnable()
		{
			if (Race.Setup.Next != null)
			{
				UpdateValue(Race.Setup.Next);
			}
		}

		[EventHandler(typeof(Race.Setup.UpdatedEvent))]
		public void OnDisplayGarage(Race.Setup.UpdatedEvent evt)
		{
			UpdateValue(evt.Value1);
		}

		private void UpdateValue(Race.Setup setup)
		{
			_label.text = TSW.Loca.DirtyLoca.GetTextValue(transform.name);
			_value.text = Game.UI.RacerPropertyHelper.GetPropertyDisplayValue(transform.name, setup.RacerDynamicProperties);
		}
	}
}