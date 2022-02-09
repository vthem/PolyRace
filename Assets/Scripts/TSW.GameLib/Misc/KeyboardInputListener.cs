using System.Collections.Generic;

using UnityEngine;

namespace TSW
{

	public class KeyboardInputListener
	{
		protected class InputEntry
		{
			public KeyCode code;
			public float value;

			public InputEntry(KeyCode code, float value)
			{
				this.code = code;
				this.value = value;
			}
		}

		private readonly float _defaultValue;
		private readonly List<InputEntry> _refEntries = new List<InputEntry>();
		private readonly LinkedList<InputEntry> _entries = new LinkedList<InputEntry>();

		public KeyboardInputListener(float defaultValue)
		{
			_defaultValue = defaultValue;
		}

		public void AddEntry(KeyCode code, float value)
		{
			_refEntries.Add(new InputEntry(code, value));
		}

		public void Update()
		{
			foreach (InputEntry entry in _refEntries)
			{
				if (Input.GetKeyDown(entry.code))
				{
					_entries.AddLast(entry);
				}
				if (Input.GetKeyUp(entry.code))
				{
					_entries.Remove(entry);
				}
			}
		}

		public float GetValue()
		{
			if (_entries.Count > 0)
			{
				return _entries.Last.Value.value;
			}
			return _defaultValue;
		}
	}
}
