using System.Collections;

namespace LevelGen.Jobs
{
	public class ChunkMeshModeler : Job
	{
		public ChunkMeshModeler(LevelProfile level) : base(level)
		{
			Weight = 3000;
			_runType = RunType.RunInThread;
		}

		protected override IEnumerator RunByStep()
		{
			_totalStep = ChunksNoSeamCount();
			foreach (Chunk chunk in ChunksNoSeam())
			{
				chunk.CreateMeshData();
				yield return null;
			}
		}
	}
}
