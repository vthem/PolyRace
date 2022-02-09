using System;

using UnityEngine;

namespace Game.Metrics
{
	public interface IProgress
	{
		float ComputeProgress(int best, int current);
		int ReverseProgress(int best, float progress);
	}

	public class BestTimeProgress : IProgress
	{
		private const float _in = 1.2f;
		public float ComputeProgress(int best, int current)
		{
			if (best == 0)
			{
				return 0f;
			}
			int max = best;
			float min = best * _in;
			float db = min - max;
			float dc = Math.Max(0, min - current);
			return dc / (float)db;
		}

		public int ReverseProgress(int best, float progress)
		{
			return Mathf.RoundToInt((best * _in) - progress * (best * _in - best));
		}
	}

	public class DefaultHigherBetterProgress : IProgress
	{
		public float ComputeProgress(int best, int current)
		{
			if (best == 0)
			{
				return 0f;
			}
			return current / (float)best;
		}

		public int ReverseProgress(int best, float progress)
		{
			return Mathf.RoundToInt(best * progress);
		}
	}

	public class DefaulLowerBetterProgress : IProgress
	{
		public float ComputeProgress(int best, int current)
		{
			if (current == 0)
			{
				return 0f;
			}
			return best / (float)current;
		}

		public int ReverseProgress(int best, float progress)
		{
			return Mathf.RoundToInt(best * progress);
		}
	}

}
