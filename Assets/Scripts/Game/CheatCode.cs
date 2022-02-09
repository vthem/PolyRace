using UnityEngine;

namespace Game
{
	public class CheatCode : MonoBehaviour
	{
		private string _current = string.Empty;

		private void OnGUI()
		{
			if (Event.current.isKey && Event.current.type == EventType.KeyDown && Event.current.keyCode != KeyCode.None)
			{
				_current += Event.current.keyCode.ToString().ToLower();
				if (_current == "dfile")
				{
					Player.PlayerManager.Instance.ClearData();
					_current = string.Empty;
					Debug.LogWarning("Clear all data");
				}
			}
		}

		private void OnEnable()
		{
			_current = string.Empty;
		}
	}
}