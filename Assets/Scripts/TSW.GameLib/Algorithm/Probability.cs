using System.Collections.Generic;

namespace TSW.Algorithm
{
	public class Probability
	{
		public static int GetProbabilityIndex(List<float> probArray, float random)
		{
			for (int i = 0; i < probArray.Count; ++i)
			{
				if (random < probArray[i])
				{
					return i;
				}
				else
				{
					random -= probArray[i];
				}
			}
			return probArray.Count - 1;
		}
	}
}
