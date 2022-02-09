using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class YourRank : MonoBehaviour
	{
		[SerializeField]
		private Text _rankValueText;

		[SerializeField]
		private Text _totalValueRank;

		//public void UpdateText(Server.ILeaderboard lb) {
		//    if (lb.HasError) {
		//        _rankValueText.text = "--";
		//        _totalValueRank.text = TSW.Loca.DirtyLoca.GetTextValue("TotalPlayer") + "\n--";
		//    } else {
		//        _rankValueText.text = lb.PlayerRank.ToString();
		//        _totalValueRank.text = TSW.Loca.DirtyLoca.GetTextValue("TotalPlayer") + "\n" + lb.TotalCount.ToString();
		//    }
		//}
	}
}