using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class LeaderboardEntry : MonoBehaviour
	{
		[SerializeField]
		private Text _rankText;

		[SerializeField]
		private Text _nameText;

		[SerializeField]
		private Text _scoreText;

		public void SetText(string rank, string name, string score)
		{
			_rankText.text = rank;
			_nameText.text = name;
			_scoreText.text = score;
		}

		public void ClearText()
		{
			_rankText.text = "--";
			_nameText.text = "--";
			_scoreText.text = "--";
		}
	}
}
