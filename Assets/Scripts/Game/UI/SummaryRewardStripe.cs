using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class SummaryRewardStripe : SummaryStripe
	{
		[SerializeField]
		private Text _rewardText;

		[SerializeField]
		private Transform[] _icons;

		[SerializeField]
		private GameObject _missionButton;
		private UnlockReward.Reward _reward;

		public override bool ShouldBeDisplayed(SummaryDisplayData data)
		{
			MissionDisplayData missionData = data as MissionDisplayData;
			if (missionData != null && missionData.Rewards != null)
			{
				return missionData.Rewards.Count > 0;
			}
			return false;
		}

		public override IEnumerable<SummaryStripe> GetDisplayInstance(SummaryDisplayData data)
		{
			MissionDisplayData missionData = data as MissionDisplayData;
			for (int i = 0; i < missionData.Rewards.Count; ++i)
			{
				SummaryRewardStripe stripe = GameObject.Instantiate(gameObject).GetComponent<SummaryRewardStripe>();
				stripe.ConfigureInstance(missionData, missionData.Rewards[i]);
				yield return stripe;
			}
		}

		protected void ConfigureInstance(SummaryDisplayData data, UnlockReward.Reward reward)
		{
			_reward = reward;
		}

		public override void Display(int index)
		{
			base.Display(index);
			_missionButton.SetActive(false);
			switch (_reward.Type)
			{
				case UnlockReward.UnlockType.DevGhost:
					_rewardText.text = TSW.Loca.DirtyLoca.GetTextValue("UnlockDevGhost");
					break;
				case UnlockReward.UnlockType.Difficulty:
					_rewardText.text = string.Format(
						TSW.Loca.DirtyLoca.GetTextValue("UnlockDifficulty"),
						TSW.Loca.DirtyLoca.GetTextValue(_reward.Id.ToString())
					);
					break;
				case UnlockReward.UnlockType.Hovercraft:
					_rewardText.text = string.Format(
						TSW.Loca.DirtyLoca.GetTextValue("UnlockHovercraft"),
						_reward.Id.ToString()
					);
					break;
				case UnlockReward.UnlockType.HovercraftColor:
					_rewardText.text = string.Format(
						TSW.Loca.DirtyLoca.GetTextValue("UnlockHovercraftColor"),
						TSW.Loca.DirtyLoca.GetTextValue("Color" + _reward.Id.ToString())
					);
					break;
				case UnlockReward.UnlockType.Mission:
					_rewardText.text = TSW.Loca.DirtyLoca.GetTextValue("UnlockMission");
					_missionButton.SetActive(true);
					break;
				case UnlockReward.UnlockType.Rank:
					_rewardText.text = string.Format(
						TSW.Loca.DirtyLoca.GetTextValue("UnlockRank"),
						TSW.Loca.DirtyLoca.GetTextValue("Rank" + _reward.Id.ToString())
					);
					break;
				case UnlockReward.UnlockType.Region:
					_rewardText.text = string.Format(
						TSW.Loca.DirtyLoca.GetTextValue("UnlockRegion"),
						TSW.Loca.DirtyLoca.GetTextValue(_reward.Id.ToString())
					);
					break;
			}
			foreach (Transform t in _icons)
			{
				Summary.EmphasisByScale(t);
			}
		}
	}
}
