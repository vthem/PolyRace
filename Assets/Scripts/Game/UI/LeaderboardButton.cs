using System;

using UnityEngine;

namespace Game.UI
{
	public class LeaderboardButton : MonoBehaviour
	{

		[SerializeField]
		private Leaderboard _leaderboardUI;

		[SerializeField]
		private ActiveSwitcher _activeSwitcher;

		public void OnLeaderboardButton()
		{
			_leaderboardUI.SetDisplayParameters(
				GetFormatter(),
				GetLocalizedLeaderboardName()
			);
			_leaderboardUI.ClearTables();
			_activeSwitcher.Back = _activeSwitcher.Active;
			_activeSwitcher.Switch(_leaderboardUI);
		}

		private Func<int, string> GetFormatter()
		{
			//if (_leaderboardId == Server.LeaderboardManager.LeaderboardId.ROTDToday ||
			//    _leaderboardId == Server.LeaderboardManager.LeaderboardId.ROTDPreviousDay) {
			//    return (score) => {
			//        return TSW.Chronometer.FormatTime(score / 1000f);
			//    };
			//}
			return null;
		}

		private string GetLocalizedLeaderboardName()
		{
			//switch (_leaderboardId) {
			//    case Game.Server.LeaderboardManager.LeaderboardId.ROTDToday:
			//        return TSW.Loca.DirtyLoca.GetTextValue("LeaderboardNameROTDToday");
			//    case Game.Server.LeaderboardManager.LeaderboardId.ROTDPreviousDay:
			//        return TSW.Loca.DirtyLoca.GetTextValue("LeaderboardNameROTDYesterday");
			//}
			return string.Empty;
		}
	}
}
