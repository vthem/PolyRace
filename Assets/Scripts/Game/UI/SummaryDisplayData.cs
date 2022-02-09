using System.Collections.Generic;

namespace Game.UI
{
	public class SummaryDisplayData
	{
		public EndRaceType EndType { get; private set; }

		public Race.Setup RaceSetup { get; private set; }

		public Metrics.Collection RaceMetrics { get; private set; }

		public void SetData(Race.Setup setup, Metrics.Collection metrics, EndRaceType endRaceType)
		{
			RaceMetrics = metrics;
			EndType = endRaceType;
			RaceSetup = setup;
		}
	}

	public class ChallengeDisplayData : SummaryDisplayData
	{
		public bool ChallengeExpired { get; private set; }
		public float ChallengeProgress { get; private set; }
		public int StarCount { get; private set; }

		public void SetChallengeData(bool expired, float progress, int startCount)
		{
			ChallengeExpired = expired;
			ChallengeProgress = progress;
			StarCount = startCount;
		}
	}

	public class ROTDDisplayData : SummaryDisplayData
	{
		public int ROTDRank { get; private set; }
		public bool RaceFinished { get; private set; }
		public void SetROTDData(int ROTDRank, bool raceFinished)
		{
			this.ROTDRank = ROTDRank;
			RaceFinished = raceFinished;
		}
	}

	public class MissionDisplayData : SummaryDisplayData
	{
		public int StarCount { get; private set; }
		public List<UnlockReward.Reward> Rewards { get; private set; }

		public void SetMissionData(int startCount, List<UnlockReward.Reward> rewards)
		{
			StarCount = startCount;
			Rewards = rewards;
		}
	}

}
