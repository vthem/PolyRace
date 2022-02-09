using TSW.Messaging;

using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class RaceTypeSelectionButton : MonoBehaviour
	{
		[SerializeField]
		private string _prefixLockKey;

		[SerializeField]
		private string _glue;

		[SerializeField]
		private Text _raceTypeText;

		private void Start()
		{
			Dispatcher.AddHandler<Race.Setup.UpdatedEvent>(OnDisplayGarage);
		}

		public void OnEnable()
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
			string raceTypeLoc = TSW.Loca.DirtyLoca.GetTextValue(Race.Setup.Next.RaceType.ToString());
			if (string.IsNullOrEmpty(_prefixLockKey))
			{
				_raceTypeText.text = raceTypeLoc;
			}
			else
			{
				_raceTypeText.text = TSW.Loca.DirtyLoca.GetTextValue(_prefixLockKey) + _glue + raceTypeLoc;
			}
		}

		public void OnMinusRaceType()
		{
			Race.Setup.Next.UpdateType((Race.RaceType)PreviousEnum((int)Race.Setup.Next.RaceType, typeof(Race.RaceType)));
			UpdateValue(Race.Setup.Next);
		}

		public void OnPlusRaceType()
		{
			Race.Setup.Next.UpdateType((Race.RaceType)NextEnum((int)Race.Setup.Next.RaceType, typeof(Race.RaceType)));
			UpdateValue(Race.Setup.Next);
		}

		private int NextEnum(int cur, System.Type enumType)
		{
			return Mathf.Abs((cur + 1) % System.Enum.GetNames(enumType).Length);
		}

		private int PreviousEnum(int cur, System.Type enumType)
		{
			if (--cur < 0)
			{
				cur = System.Enum.GetNames(enumType).Length - 1;
			}
			return cur;
		}
	}
}