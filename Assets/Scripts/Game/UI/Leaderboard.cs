using System;

using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class Leaderboard : UIElement, INavigationOption
	{
		[SerializeField]
		private Text _rankText;

		[SerializeField]
		private Text _totalText;

		[SerializeField]
		private LeaderboardTable _globalTable;

		[SerializeField]
		private LeaderboardTable _userTable;

		[SerializeField]
		private Text _leaderboardName;

		public void SetDisplayParameters(Func<int, string> scoreFormatter, string leaderboardName)
		{
			_globalTable.ScoreFormatter = scoreFormatter;
			_userTable.ScoreFormatter = scoreFormatter;
			_leaderboardName.text = leaderboardName;
		}

		//      public void UpdateEntry(Server.LeaderboardEntryType type, int index, int rank, string name, int score, string data) {
		//          string fullname = name;
		//          switch (type) {
		//          case Server.LeaderboardEntryType.Top:
		//              _globalTable.UpdateEntry(index, rank, fullname, score, data);
		//              break;
		//          case Server.LeaderboardEntryType.Around:
		//              _userTable.UpdateEntry(index, rank, fullname, score, data);
		//              break;
		//          }
		//      }

		//public void UpdateData(Server.ILeaderboard leaderboard) {
		//	if (!leaderboard.HasError) {
		//		_rankText.text = leaderboard.PlayerRank.ToString();
		//		_totalText.text = leaderboard.TotalCount.ToString();
		//	}
		//}

		public void ClearTables()
		{
			_globalTable.ClearAllLines();
			_userTable.ClearAllLines();
		}

		public string[] GetActiveButton()
		{
			return new string[] { "ExitLeaderboard_Button" };
		}

		public string GetFocusButton()
		{
			return string.Empty;
		}
	}
}