using System.Collections.Generic;

using UnityEngine;

namespace TSW.Unity
{
	public class DrawLine : TSW.Design.USingleton<DrawLine>
	{
		private struct Line
		{
			public Vector3 from;
			public Vector3 to;
			public Color color;

			public Line(Vector3 from, Vector3 to, Color color)
			{
				this.from = from;
				this.to = to;
				this.color = color;
			}
		}

		private class LineSet
		{
			public List<Line> lines;
			public bool active;

			public LineSet(List<Line> lines, bool active)
			{
				this.lines = lines;
				this.active = active;
			}
		}

		private readonly Dictionary<string, LineSet> _linesByTag = new Dictionary<string, LineSet>();

		public static void Add(Vector3 from, Vector3 to, string tag, Color color)
		{
			LineSet ls;
			if (!Instance._linesByTag.TryGetValue(tag, out ls))
			{
				ls = new LineSet(new List<Line>(), true);
				Instance._linesByTag[tag] = ls;
			}
			ls.lines.Add(new Line(from, to, color));
		}

		public static List<string> GetAllTags()
		{
			return new List<string>(Instance._linesByTag.Keys);
		}

		public static bool IsTagActive(string tag)
		{
			LineSet ls;
			if (!Instance._linesByTag.TryGetValue(tag, out ls))
			{
				return false;
			}
			return ls.active;
		}

		public static void SetStateByTag(string tag, bool state)
		{
			LineSet ls;
			if (Instance._linesByTag.TryGetValue(tag, out ls))
			{
				ls.active = state;
			}
		}

		public static void Clear(string tag)
		{
			if (Instance._linesByTag.ContainsKey(tag))
			{
				Instance._linesByTag.Remove(tag);
			}
		}

		public static void ClearAll()
		{
			Instance._linesByTag.Clear();
		}

		private void OnDrawGizmos()
		{
			foreach (KeyValuePair<string, LineSet> keyValue in _linesByTag)
			{
				if (keyValue.Value.active)
				{
					foreach (Line line in keyValue.Value.lines)
					{
						Gizmos.color = line.color;
						Gizmos.DrawLine(line.from, line.to);
					}
				}
			}
		}
	}
}
