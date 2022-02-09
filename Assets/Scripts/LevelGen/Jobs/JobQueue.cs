using System;
using System.Collections;
using System.Collections.Generic;

using TSW;

namespace LevelGen.Jobs
{
	public class JobQueue
	{
		private readonly string _name;
		private readonly List<Job> _jobs = new List<Job>();
		private DedicatedCoroutine _runCoroutine;
		private Job _currentJob;
		private Action<List<Chunk>> _onEndAction;

		public float Progress { get; private set; }
		public string WorkStep { get; private set; }

		public JobQueue(string name)
		{
			_name = name;
		}

		public void Add(Job job)
		{
			_jobs.Add(job);
		}

		public void Run(List<Chunk> chunks, int nSeamChunk)
		{
			TSW.Profiler.Clear();
			TSW.Profiler.ProfilerEntry global = TSW.Profiler.AddEntry("JobQueue: " + _name);
			if (chunks.Count > 0)
			{
				Log("Starting job queue:" + _name + " with #" + chunks.Count + " chunks" + " from index:" + chunks[0].Index + " to index:" + chunks[chunks.Count - 1].Index);
			}
			else
			{
				Log("Starting job queue:" + _name + " with #" + chunks.Count + " chunks" + " from index:empty to index:-");
			}

			foreach (Job job in _jobs)
			{
				Log("Starting job: " + job.GetType().Name + " progress:" + Progress);
				TSW.Profiler.ProfilerEntry pe = TSW.Profiler.AddEntry(job.GetType().Name);
				job.Run(chunks, nSeamChunk);
				pe.End();
				Log("End job: " + job.GetType().Name + " progress:" + Progress);
			}
			global.End();
			Log(TSW.Profiler.Instance.ToString());
		}

		public void RunAsync(List<Chunk> chunks, int nSeamChunk, Action<List<Chunk>> onEndAction)
		{
			_onEndAction = onEndAction;
			_runCoroutine = DedicatedCoroutineProvider.NewRoutine(AsyncRunCoroutine(chunks, nSeamChunk), "JobQueue");
		}

		public void Stop()
		{
			if (_runCoroutine != null)
			{
				UnityEngine.GameObject.DestroyImmediate(_runCoroutine);
				_runCoroutine = null;
			}
			if (_currentJob != null)
			{
				UnityEngine.Debug.Log("Job running: " + _currentJob.ToString());
				_currentJob.Stop();
			}
		}

		private IEnumerator AsyncRunCoroutine(List<Chunk> chunks, int nSeamChunk)
		{
			TSW.Profiler.Clear();
			TSW.Profiler.ProfilerEntry global = TSW.Profiler.AddEntry("JobQueue: " + _name);
			if (chunks.Count > 0)
			{
				Log("Starting job queue:" + _name + " with #" + chunks.Count + " chunks" + " from index:" + chunks[0].Index + " to index:" + chunks[chunks.Count - 1].Index);
			}
			else
			{
				Log("Starting job queue:" + _name + " with #" + chunks.Count + " chunks" + " from index:empty to index:-");
			}

			float totalWeight = 0f;
			foreach (Job job in _jobs)
			{
				totalWeight += job.Weight;
			}
			// add 5% for OnEndAction
			totalWeight += totalWeight * 0.05f;

			float currentWeight = 0f;

			for (int i = 0; i < _jobs.Count; ++i)
			{
				_currentJob = _jobs[i];
				TSW.Profiler.ProfilerEntry pe = TSW.Profiler.AddEntry(_currentJob.GetType().Name);
				WorkStep = _currentJob.GetType().Name;
				Log("Starting job: " + _currentJob.GetType().Name + " progress:" + Progress);
				_currentJob.RunAsync(chunks, nSeamChunk);
				while (!_currentJob.Ended)
				{
					Progress = (currentWeight + _currentJob.Progress) / totalWeight;
					yield return null;
				}
				Progress = (currentWeight + _currentJob.Progress) / totalWeight;
				Log("End job: " + _currentJob.GetType().Name + " progress:" + Progress);
				currentWeight += _currentJob.Weight;
				_currentJob = null;
				pe.End();
			}
			global.End();
			Log(TSW.Profiler.Instance.ToString());

			Log("Ending job queue:" + _name + " calling onEndAction:" + (_onEndAction != null));
			if (_onEndAction != null)
			{
				_onEndAction(chunks);
			}
			Progress = 1f;
			yield return null;
			_runCoroutine.gameObject.Destroy();
		}

		private static void Log(string text)
		{
			//			UnityEngine.Debug.Log("JobQueue/ " + text);
		}
	}
}
