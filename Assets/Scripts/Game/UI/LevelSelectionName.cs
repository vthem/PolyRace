using TSW.Messaging;

using UnityEngine;

namespace Game.UI
{
	public class LevelSelectionName : MonoBehaviour
	{
		[SerializeField]
		private string _prefixLockKey;

		[SerializeField]
		private string _glue;

		private void Start()
		{
			Dispatcher.AddHandler<Race.Setup.UpdatedEvent>(OnDisplayGarage);
		}

		private void OnEnable()
		{
			if (Race.Setup.Next != null)
			{
				UpdateValue(Race.Setup.Next);
			}
			else
			{
				Debug.Log("NEXT NOT SET!!");
			}
		}

		public void OnDisplayGarage(Race.Setup.UpdatedEvent evt)
		{
			UpdateValue(evt.Value1);
		}

		private void UpdateValue(Race.Setup setup)
		{
			UnityEngine.UI.Text text = GetComponent<UnityEngine.UI.Text>();
			string locRegion = TSW.Loca.DirtyLoca.GetTextValue(Race.Setup.Next.LevelIdentifier.LevelRegion.ToString());
			string locDifficulty = TSW.Loca.DirtyLoca.GetTextValue(Race.Setup.Next.LevelIdentifier.LevelDifficulty.ToString());
			if (string.IsNullOrEmpty(_prefixLockKey))
			{
				text.text = locRegion + " / " + locDifficulty;
			}
			else
			{
				text.text = TSW.Loca.DirtyLoca.GetTextValue(_prefixLockKey) + _glue + locRegion + " / " + locDifficulty;
			}
		}
	}
}