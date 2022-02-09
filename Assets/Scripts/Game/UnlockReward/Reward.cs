using System.Collections.Generic;

namespace Game.UnlockReward
{
	public struct Reward
	{
		public RewardId Id { get; private set; }
		public UnlockType Type { get; private set; }

		public Reward(RewardId id, UnlockType type)
		{
			Id = id;
			Type = type;
		}

		public static Dictionary<RewardId, Reward> ById = new Dictionary<RewardId, Reward>() {
			{ RewardId.Yellow, new Reward(RewardId.Yellow, UnlockType.HovercraftColor) },
			{ RewardId.Arctic, new Reward(RewardId.Arctic, UnlockType.Region) },
			{ RewardId.Black, new Reward(RewardId.Black, UnlockType.HovercraftColor) },
			{ RewardId.Continental, new Reward(RewardId.Continental, UnlockType.Region) },
			{ RewardId.Cyan, new Reward(RewardId.Cyan, UnlockType.HovercraftColor) },
			{ RewardId.DevGhost, new Reward(RewardId.DevGhost, UnlockType.DevGhost) },
			{ RewardId.Extreme, new Reward(RewardId.Extreme, UnlockType.Difficulty) },
			{ RewardId.Green, new Reward(RewardId.Green, UnlockType.HovercraftColor) },
			{ RewardId.Grey, new Reward(RewardId.Grey, UnlockType.HovercraftColor) },
			{ RewardId.Hard, new Reward(RewardId.Hard, UnlockType.Difficulty) },
			{ RewardId.Mission, new Reward(RewardId.Mission, UnlockType.Mission) },
			{ RewardId.Orange, new Reward(RewardId.Orange, UnlockType.HovercraftColor) },
			{ RewardId.Tracer, new Reward(RewardId.Tracer, UnlockType.Hovercraft) },
			{ RewardId.TrackR, new Reward(RewardId.TrackR, UnlockType.Hovercraft) },
			{ RewardId.TRex, new Reward(RewardId.TRex, UnlockType.Hovercraft) },
			{ RewardId.Adept, new Reward(RewardId.Adept, UnlockType.Rank) },
			{ RewardId.Skilled, new Reward(RewardId.Skilled, UnlockType.Rank) },
			{ RewardId.Master, new Reward(RewardId.Master, UnlockType.Rank) },
			{ RewardId.Grandmaster, new Reward(RewardId.Grandmaster, UnlockType.Rank) },
		};

		public override string ToString()
		{
			return string.Format("Reward Id:" + Id + " Type:" + Type);
		}
	}
}