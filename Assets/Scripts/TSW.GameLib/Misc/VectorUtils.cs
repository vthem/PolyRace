using System.Collections.Generic;

using UnityEngine;

namespace TSW
{
	public static class VectorUtils
	{
		public static Vector3 Right(this Vector3 forward, Vector3 up)
		{
			return Vector3.Cross(up.normalized, forward.normalized);
		}

		public static Vector3 Up(this Vector3 forward, Vector3 right)
		{
			return Vector3.Cross(forward.normalized, right.normalized);
		}

		public static bool MultiCast(Vector3 direction, float distance, out RaycastHit hit, int layerMask, params Vector3[] origins)
		{
			for (int i = 0; i < origins.Length; ++i)
			{
				//DebugDrawLine.DrawLine(origins[i], origins[i] + direction * distance, Color.cyan);
				if (Physics.Raycast(origins[i], direction, out hit, distance, layerMask))
				{
					return true;
				}
			}
			hit = new RaycastHit();
			return false;
		}

		public static int ClosestPoint(Vector3 point, Vector3[] points)
		{
			int closest = 0;
			float minMagnitude = float.MaxValue;
			for (int i = 0; i < points.Length; ++i)
			{
				float magnitude = Mathf.Abs((points[i] - point).magnitude);
				if (magnitude < minMagnitude)
				{
					closest = i;
					minMagnitude = magnitude;
				}
			}
			return closest;
		}

		public static IEnumerable<Vector3> BezierIterator(Vector3 p1, Vector3 cp1, Vector3 p2, Vector3 cp2, float step)
		{
			if (step == 0f)
			{
				yield break;
			}
			for (float t = step; t < 1f; t += step)
			{
				Vector3 c1 = -3 * p1 + 3 * cp1;
				Vector3 c2 = 3 * p1 - 6 * cp1 + 3 * cp2;
				Vector3 c3 = -p1 + 3 * cp1 - 3 * cp2 + p2;
				float t2 = t * t;
				float t3 = t2 * t;
				yield return p1 + t * c1 + t2 * c2 + t3 * c3;
			}
		}

		public static IEnumerable<Vector3> Range(Vector3 from, Vector3 to, float step)
		{
			return Range(from, (to - from).normalized, step, (to - from).magnitude);
		}

		public static IEnumerable<Vector3> Range(Vector3 from, Vector3 dir, float step, float length)
		{
			float magnitude = 0f;
			while (magnitude < length)
			{
				yield return from + dir * magnitude;
				magnitude += step;
			}
		}
	}
}