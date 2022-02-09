using LevelGen;

using UnityEngine;

namespace Game
{
	public class LevelActivator : TSW.Design.USingleton<LevelActivator>
	{
		private Transform _target;
		private Level _level;
		private LevelBuilder _builder;

		public void Initialize(GameObject levelObject, Transform target, LevelBuilder builder)
		{
			if (levelObject == null)
			{
				Debug.LogWarning("Invalid parameter levelObject : parameter is null");
				return;
			}
			if (target == null)
			{
				Debug.LogWarning("Invalid parameter target : parameter is null");
				return;
			}
			if (builder == null)
			{
				Debug.LogWarning("Invalid parameter builder : parameter is null");
				return;
			}
			if (builder.IsStatic)
			{
				enabled = false;
				return;
			}
			_target = target;
			_level = builder.Level;
			_builder = builder;
			OcclusionUpdate(0);
			InvokeRepeating("CheckPositionUpdate", 0f, .5f);
		}

		public void Stop()
		{
			CancelInvoke();
		}

		private void CheckPositionUpdate()
		{
			if (_level == null)
			{
				Debug.LogWarning("CheckPositionUpdate - _level is null");
				return;
			}
			if (_target == null)
			{
				Debug.LogWarning("CheckPositionUpdate - _target is null");
				return;
			}
			Chunk chunk = _level.GetChunkByPosition(_level.Profile.WorldToChunk(_target.position));
			if (chunk == null)
			{
				Debug.LogWarning("CheckPositionUpdate - chunk is null");
				return;
			}
			OcclusionUpdate(chunk.Index);
			_builder.LoadNextSegmentIfRequired(chunk.Index);
		}

		private void OcclusionUpdate(int index)
		{
			int activateStart = Mathf.Max(0, index - 3);
			int activateEnd = Mathf.Min(_level.Chunks.Count, index + 3);
			ChangeObjectState(0, activateStart, false);
			ChangeObjectState(activateStart, activateEnd, true);
			ChangeObjectState(activateEnd, _level.Chunks.Count, false);
		}

		private static readonly string[] _types = new string[] {
			LevelObjectType.Gate.ToString(),
			LevelObjectType.Arrow.ToString(),
			LevelObjectType.Obstacle.ToString(),
			LevelObjectType.Scenery.ToString()
		};

		private void ChangeObjectState(int from, int to, bool state)
		{
			for (int i = from; i < to; ++i)
			{
				ChangeContainerState(_level.GetChunkDynamicContainer(i), state);
				ChangeContainerState(_level.GetChunkStaticContainer(i), state);
			}
		}

		private void ChangeContainerState(GameObject obj, bool state)
		{
			if (obj == null)
			{
				return;
			}
			for (int t = 0; t < _types.Length; ++t)
			{
				Transform objTransform = obj.transform.Find(_types[t]);
				if (null == objTransform)
				{
					continue;
				}
				if (objTransform.gameObject.activeSelf != state)
				{
					objTransform.gameObject.SetActive(state);
				}
			}
		}
	}
}
