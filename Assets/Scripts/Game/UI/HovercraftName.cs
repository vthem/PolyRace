using TSW.Messaging;

using UnityEngine;

namespace Game.UI
{
	public class HovercraftName : MonoBehaviour
	{
		[SerializeField]
		private string _prefixLockKey;

		[SerializeField]
		private string _glue;

		private void Awake()
		{
			Dispatcher.AddHandler<Race.Setup.UpdatedEvent>(OnDisplayGarage);
		}

		private void OnEnable()
		{
			if (Race.Setup.Next != null)
			{
				UpdateValue(Race.Setup.Next);
			}
		}

		public void OnDisplayGarage(Race.Setup.UpdatedEvent evt)
		{
			UpdateValue(evt.Value1);
		}

		private void UpdateValue(Race.Setup setup)
		{
			UnityEngine.UI.Text text = GetComponent<UnityEngine.UI.Text>();
			if (string.IsNullOrEmpty(_prefixLockKey))
			{
				text.text = setup.RacerDynamicProperties.Properties.HovercraftName;
			}
			else
			{
				text.text = TSW.Loca.DirtyLoca.GetTextValue(_prefixLockKey) + _glue + setup.RacerDynamicProperties.Properties.HovercraftName;
			}
		}
	}
}
