using System.Collections;

namespace LevelGen.Jobs
{
	public class ChunkMeshCreator : Job
	{
		private readonly TidyGameObjectDelegate _addChildObject;

		public ChunkMeshCreator(LevelProfile level, TidyGameObjectDelegate addChildObject) : base(level)
		{
			Weight = 120f;
			_runType = RunType.RunInCoroutine;
			_addChildObject = addChildObject;
		}

		protected override IEnumerator RunByStep()
		{
			_totalStep = 0;
			foreach (Chunk chunk in ChunksNoSeam())
			{
				_totalStep += chunk.GetNumberOfMesh();
			}
			foreach (Chunk chunk in ChunksNoSeam())
			{
				Log("Create meshes of chunk:" + chunk.Index);
				foreach (UnityEngine.GameObject go in chunk.CreateMesh())
				{
					_addChildObject(go, LevelObjectType.Terrain, false);
					yield return null;
				}
			}
		}
	}
}
