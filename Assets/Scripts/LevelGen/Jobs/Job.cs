using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

using TSW;

using UnityEngine;

namespace LevelGen.Jobs
{
	public class Job
	{
		public delegate void TidyGameObjectDelegate(GameObject obj, LevelObjectType type, bool isDynamic);

		public float Weight { get; protected set; }
		public float Progress { get; private set; }
		public bool Ended { get; private set; }

		protected float _totalStep;
		protected RunType _runType;
		protected bool _onEnd;
		protected List<Chunk> _chunks;
		protected LevelProfile _levelProfile;
		private int _nSeamChunk = 0;
		private DedicatedCoroutine _runCoroutine;

		protected enum RunType
		{
			RunInThread,
			RunInCoroutine
		}

		public Job(LevelProfile levelProfile)
		{
			_levelProfile = levelProfile;
		}

		public void RunAsync(List<Chunk> chunks, int nSeamChunk)
		{
			_nSeamChunk = nSeamChunk;
			_chunks = chunks;
			Progress = 0f;
			Ended = false;
			if (_runType == RunType.RunInCoroutine)
			{
				_runCoroutine = DedicatedCoroutineProvider.NewRoutine(RunInCoroutine(), GetType().Name);
			}
			else if (_runType == RunType.RunInThread)
			{
				Thread thread = new Thread(new ThreadStart(RunInThread));
				thread.Start();
			}
		}

		public void Stop()
		{
			if (_runCoroutine != null)
			{
				_runCoroutine.StopAllCoroutines();
				_runCoroutine = null;
				UnityEngine.Debug.Log("Stop job " + ToString());
			}
		}

		public override string ToString()
		{
			return GetType().ToString();
		}

		public void Run(List<Chunk> chunks, int nSeamChunk)
		{
			_chunks = chunks;
			_nSeamChunk = nSeamChunk;
			IEnumerator enumerator = RunByStep();
			while (enumerator.MoveNext())
			{
				;
			}
		}

		protected virtual IEnumerator RunByStep()
		{
			yield break;
		}

		protected void Log(string text)
		{
			TSW.Log.Logger.Add(GetType().Name + "/ " + text);
		}

		private IEnumerator RunInCoroutine()
		{
			Log("Starting coroutine in job " + GetType().Name);
			float step = 0f;
			Stopwatch timer = Stopwatch.StartNew();
			Stopwatch timerMax = Stopwatch.StartNew();
			long maxStepDuration = 0;
			Progress = (step / _totalStep) * Weight;
			IEnumerator enumerator = RunByStep();
			while (enumerator.MoveNext())
			{
				maxStepDuration = maxStepDuration > timerMax.ElapsedMilliseconds ? maxStepDuration : timerMax.ElapsedMilliseconds;
				Progress = (++step / _totalStep) * Weight;
				if (timer.ElapsedMilliseconds > 2)
				{
					Log("Yield in job " + GetType().Name + " ended:" + Ended + " " + Progress + "/" + Weight);
					yield return null;
					timer = Stopwatch.StartNew();
				}
				timerMax = Stopwatch.StartNew();
			}
			if (maxStepDuration > 5)
			{
				Log("Warning: Max step duration:" + maxStepDuration + " ms in job " + GetType().Name);
			}

			Progress = Weight;
			Ended = true;

			yield return null;
			_runCoroutine.gameObject.Destroy();
		}

		private void RunInThread()
		{
			try
			{
				IEnumerator enumerator = RunByStep();
				float step = 0f;
				while (enumerator.MoveNext())
				{
					Progress = (++step / _totalStep) * Weight;
				}
			}
			catch (System.Exception ex)
			{
				Log("Fail to run thread job\n" + ex.Message + "\n--\n" + ex.StackTrace);
			}
			Ended = true;
		}

		protected IEnumerable<Chunk> Chunks()
		{
			return _chunks;
		}

		protected IEnumerable<Chunk> ChunksNoSeam()
		{
			for (int i = 0; i < _chunks.Count - _nSeamChunk; ++i)
			{
				yield return _chunks[i];
			}
			for (int n = _nSeamChunk; n > 0; --n)
			{
				if (_chunks[_chunks.Count - n].Index == _levelProfile.ShapeLength - n)
				{
					yield return _chunks[_chunks.Count - n];
				}
			}
		}

		protected int ChunksNoSeamCount()
		{
			int count = 0;
			for (int i = 0; i < _chunks.Count - _nSeamChunk; ++i)
			{
				count++;
			}
			for (int n = _nSeamChunk; n > 0; --n)
			{
				if (_chunks[_chunks.Count - n].Index == _levelProfile.ShapeLength - n)
				{
					count++;
				}
			}
			return count;
		}
	}
}
