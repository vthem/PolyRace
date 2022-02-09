using System;

using UnityEngine;

namespace Game.UI
{
	public class LeaderboardTable : MonoBehaviour
	{
		[SerializeField]
		private Transform _rowGroup;

		public Func<int, string> ScoreFormatter { get; set; }

		public void UpdateEntry(int index, int rank, string name, int score, string data)
		{
			if (index >= _rowGroup.childCount)
			{
				Debug.LogWarning("Invalid index:" + index);
				return;
			}
			Transform child = _rowGroup.GetChild(index);
			if (null != child)
			{
				string fullname = name;
				if (data.Contains(":demo"))
				{
					fullname = "[Demo] " + name;
				}
				child.GetComponent<LeaderboardEntry>().SetText(rank.ToString(), fullname, ScoreFormatter(score));
			}
		}

		public void ClearAllLines()
		{
			for (int i = 0; i < _rowGroup.childCount; ++i)
			{
				Transform child = _rowGroup.GetChild(i);
				if (null != child)
				{
					child.GetComponent<LeaderboardEntry>().ClearText();
				}
			}
		}
	}
}
