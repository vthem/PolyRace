using System.Collections.Generic;

using TSW;
using TSW.Unity;

using UnityEngine;

namespace LevelGen
{
	public class LevelDebug
	{
		public static void DrawPath(List<Vector3> path, Color color, string tag)
		{
			for (int i = 1; i < path.Count; ++i)
			{
				Vector3 v1 = path[i - 1];
				Vector3 v2 = path[i];
				v1.y += 20f;
				v2.y += 20f;
				DrawLine.Add(v1, v2, tag, color);
			}
		}

		public static void DrawPathPoint(List<Vector3> path, Color color, string tag)
		{
			for (int i = 0; i < path.Count; ++i)
			{
				DrawLine.Add(path[i], path[i] + Vector3.up * 20f, tag, color);
			}
		}

		public static void DrawBezierPoint(Vector3 p1, Vector3 cp1, Vector3 p2, Vector3 cp2, string tag, Color color)
		{
			p1 += Vector3.up * 20f;
			p2 += Vector3.up * 20f;
			cp1 += Vector3.up * 20f;
			cp2 += Vector3.up * 20f;
			DrawLine.Add(p1, p2, tag, color);
			DrawLine.Add(p1, cp1, tag, Color.red);
			DrawLine.Add(p2, cp2, tag, Color.green);
		}

		public static void DrawBezierSegment(Vector3 p1, Vector3 cp1, Vector3 p2, Vector3 cp2, string tag, Color color)
		{
			p1 += Vector3.up * 20f;
			p2 += Vector3.up * 20f;
			cp1 += Vector3.up * 20f;
			cp2 += Vector3.up * 20f;
			Vector3 last = p1;
			foreach (Vector3 p in VectorUtils.BezierIterator(p1, cp1, p2, cp2, .1f))
			{
				DrawLine.Add(last, p, tag, color);
				last = p;
			}
			DrawLine.Add(last, p2, tag, color);
		}
	}
}
