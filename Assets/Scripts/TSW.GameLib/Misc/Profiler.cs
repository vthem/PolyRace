using System.Collections.Generic;
using System.Diagnostics;

using TSW.Design;

namespace TSW
{
	public class Profiler : Singleton<Profiler>
	{
		private readonly List<ProfilerEntry> _profileEntries = new List<ProfilerEntry>();

		public class ProfilerEntry
		{
			public string Text { get; private set; }
			public int ElabsedValue => (int)_stopWatch.ElapsedMilliseconds;

			private readonly Stopwatch _stopWatch;


			public ProfilerEntry(string text)
			{
				Text = text;
				_stopWatch = Stopwatch.StartNew();
			}

			public void End()
			{
				_stopWatch.Stop();
			}

			public override string ToString()
			{
				return Text + " = " + _stopWatch.ElapsedMilliseconds.ToString();
			}
		}

		public static void Clear()
		{
			Instance._profileEntries.Clear();
		}

		public static ProfilerEntry AddEntry(string text)
		{
			ProfilerEntry entry = new ProfilerEntry(text);
			lock (Instance._profileEntries)
			{
				Instance._profileEntries.Add(entry);
			}
			return entry;
		}

		public override string ToString()
		{
			string s = "=== Profiler Summary ===\n";
			lock (Instance._profileEntries)
			{
				if (_profileEntries.Count > 0)
				{
					HashSet<string> entryNames = new HashSet<string>();
					for (int i = 0; i < _profileEntries.Count; ++i)
					{
						entryNames.Add(_profileEntries[i].Text);
					}
					foreach (string entryName in entryNames)
					{
						int max = int.MinValue;
						int min = int.MaxValue;
						int average = 0;
						int num = 0;
						for (int i = 0; i < _profileEntries.Count; ++i)
						{
							if (_profileEntries[i].Text == entryName)
							{
								max = System.Math.Max(_profileEntries[i].ElabsedValue, max);
								min = System.Math.Min(_profileEntries[i].ElabsedValue, min);
								average = (int)System.Math.Round((_profileEntries[i].ElabsedValue + average) / 2f);
								num++;
							}
						}
						s += string.Format("{0, -30} max: {1, 6} ms | min: {2, 6} ms | avg: {3, 6} ms | sample {4, 6}\n",
										   entryName, max, min, average, num);
					}
					//					for (int i = 1; i < _profileEntries.Count; ++i) {
					//						if (currentEntry != _profileEntries[i].Text) {
					//							max = _profileEntries[i].ElabsedValue;
					//							min = _profileEntries[i].ElabsedValue;
					//							average = _profileEntries[i].ElabsedValue;
					//							num = 1;
					//							currentEntry = _profileEntries[i].Text;
					//						} else {
					//							max = System.Math.Max(_profileEntries[i].ElabsedValue, max);
					//							min = System.Math.Min(_profileEntries[i].ElabsedValue, min);
					//							average = (int)System.Math.Round((_profileEntries[i].ElabsedValue + average) / 2f);
					//							num++;
					//						}
					//					}
					//					s += string.Format("{0, -30} max: {1, 6} ms | min: {2, 6} ms | avg: {3, 6} ms | sample {4, 6}\n",
					//					                   currentEntry, max, min, average, num);
				}
			}
			return s;
		}
	}
}
