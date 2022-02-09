using System.Collections.Generic;

using UnityEngine;

namespace LevelGen.Jobs
{
	public class ObjectCreator
	{
		public static Rect CombineSurface(List<Rect> surfaces)
		{
			Rect comb = new Rect
			{
				xMin = float.MaxValue,
				yMin = float.MaxValue,
				xMax = float.MinValue,
				yMax = float.MinValue
			};
			foreach (Rect surf in surfaces)
			{
				comb.xMin = Mathf.Min(surf.xMin, comb.xMin);
				comb.xMax = Mathf.Max(surf.xMax, comb.xMax);
				comb.yMin = Mathf.Min(surf.yMin, comb.yMin);
				comb.yMax = Mathf.Max(surf.yMax, comb.yMax);
			}
			return comb;
		}

		public static IEnumerable<Vector3> InSurfaceDistribution(Rect surface, float step, float y)
		{
			for (float x = step + surface.xMin; x < surface.xMax; x += step)
			{
				for (float z = step + surface.yMin; z < surface.yMax; z += step)
				{
					yield return new Vector3(x, y, z);
				}
			}
		}
	}
}
