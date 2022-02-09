using System.Collections;

namespace LevelGen.Jobs
{
	public class ChunkDataCreator : Job
	{
		public ChunkDataCreator(LevelProfile level) : base(level)
		{
			Weight = 185;
			_runType = RunType.RunInThread;
		}

		protected override IEnumerator RunByStep()
		{
			_totalStep = _chunks.Count;
			TSW.Log.Logger.Add("Running DataJobBuilder start:" + _chunks[0].Index + " last:" + _chunks[_chunks.Count - 1].Index);
			foreach (Chunk chunk in _chunks)
			{
				chunk.CreateData();
				yield return null;
			}
		}
	}
}
