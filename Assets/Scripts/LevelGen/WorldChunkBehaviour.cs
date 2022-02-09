using System.Collections.Generic;

using TSW;

using UnityEngine;

namespace LevelGen
{
	public class WorldChunkBehaviour : MonoBehaviour
	{
		[SerializeField]
		private int _inDirection;

		[SerializeField]
		private int _outDirection;

		[SerializeField]
		private LevelProfile _levelProfile;

		[SerializeField]
		private string _seedString;

		[SerializeField]
		private Transform _probe;

		[SerializeField]
		private float _probeSize;

		[SerializeField]
		private bool _dig;

		[SerializeField]
		private Chunk.Pattern _pattern;

		public void Create()
		{
			_levelProfile.Initialize(_seedString, -1);
			_levelProfile.Terrain.Initialize(_levelProfile.SeedInt);
			TSW.Profiler.Clear();
			TSW.Profiler.ProfilerEntry global = TSW.Profiler.AddEntry("ChunkTest");

			Chunk chunk = new Chunk(new Vector3(0, 0, 0), _levelProfile, _inDirection, _outDirection, 0, _pattern);
			List<Chunk> chunks = new List<Chunk>();

			//			chunks.Add(new Chunk(new Vector3(0, 0, _levelProfile.Terrain.ChunkSizeXZ), _levelProfile, 2, 4, 0, modifier));
			//			chunks.Add(new Chunk(new Vector3(_levelProfile.Terrain.ChunkSizeXZ, 0, 0), _levelProfile, 3, 4, 0, modifier));
			//			chunks.Add(new Chunk(new Vector3(0, 0, -_levelProfile.Terrain.ChunkSizeXZ), _levelProfile, 0, 4, 0, modifier));
			//			chunks.Add(new Chunk(new Vector3(-_levelProfile.Terrain.ChunkSizeXZ, 0, 0), _levelProfile, 1, 4, 0, modifier));

			foreach (Chunk c in chunks)
			{
				chunk.SetNeighboor(c);
			}
			chunk.CreateData();
			foreach (Chunk c in chunks)
			{
				c.CreateData();
			}

			if (_probe != null && _dig)
			{
				foreach (Vector3 vz in VectorUtils.Range(_probe.position, _probe.position + Vector3.forward * _probeSize, _levelProfile.Terrain.BlockSize))
				{
					foreach (Vector3 vx in VectorUtils.Range(vz, vz + Vector3.right * _probeSize, _levelProfile.Terrain.BlockSize))
					{
						chunk.SetTunnelData(chunk.LocalBlockPosition(_levelProfile.WorldToBlock(vx)));
					}
				}
			}
			chunk.CreateMeshData();
			foreach (Chunk c in chunks)
			{
				c.CreateMeshData();
			}

			Clear();
			GameObject lvl = new GameObject("Level");
			foreach (GameObject go in chunk.CreateMesh())
			{
				go.transform.SetParent(lvl.transform);
			}
			foreach (Chunk c in chunks)
			{
				foreach (GameObject go in c.CreateMesh())
				{
					go.transform.SetParent(lvl.transform);
				}
			}


			//			foreach (Int2 pos in chunk.Blocks()) {
			//				var v = level.BlockToWorld(chunk.WorldBlockPosition(pos));
			//				DrawLine.Add(v, v + Vector3.up * chunk.GetWeight(pos) * 20f, "weight", Color.cyan);
			//			}

			//			List<Rect> surfaces = chunk.GetSurfaces();
			//			Vector3[] surfPoints = new Vector3[4];
			//			Color[] colors = new Color[] {
			//				Color.red,
			//				Color.blue,
			//				Color.grey,
			//				Color.magenta,
			//				Color.yellow
			//			};
			//			int index = 0;
			//			foreach (Rect surf in surfaces) {
			//				Debug.Log("surf:" + surf + " xMin:" + surf.xMin + " xMax:" + surf.xMax + " yMin:" + surf.yMin + " yMax:" + surf.yMax);
			//				surfPoints[0] = new Vector3(surf.xMin, 100f, surf.yMin);
			//				surfPoints[1] = new Vector3(surf.xMax, 100f, surf.yMin);
			//				surfPoints[2] = new Vector3(surf.xMax, 100f, surf.yMax);
			//				surfPoints[3] = new Vector3(surf.xMin, 100f, surf.yMax);
			//				DrawLine.Add(surfPoints[0], surfPoints[1], "surf", colors[index]);
			//				DrawLine.Add(surfPoints[1], surfPoints[2], "surf", colors[index]);
			//				DrawLine.Add(surfPoints[2], surfPoints[3], "surf", colors[index]);
			//				DrawLine.Add(surfPoints[3], surfPoints[0], "surf", colors[index]);
			//				index++;
			//			}
			//
			//			Rect global = CombineSurface(surfaces);
			//			surfPoints[0] = new Vector3(global.xMin, 100f, global.yMin);
			//			surfPoints[1] = new Vector3(global.xMax, 100f, global.yMin);
			//			surfPoints[2] = new Vector3(global.xMax, 100f, global.yMax);
			//			surfPoints[3] = new Vector3(global.xMin, 100f, global.yMax);
			//			DrawLine.Add(surfPoints[0], surfPoints[1], "surf", colors[index]);
			//			DrawLine.Add(surfPoints[1], surfPoints[2], "surf", colors[index]);
			//			DrawLine.Add(surfPoints[2], surfPoints[3], "surf", colors[index]);
			//			DrawLine.Add(surfPoints[3], surfPoints[0], "surf", colors[index]);
			//			foreach (var p in InSurfaceDistribution(global, 50f, 100f)) {
			//				DrawLine.Add(p, p + Vector3.up * 10f, "distrib", colors[index]);
			//
			//				Vector3 p2 = p;
			//				Vector2 rv = Random.insideUnitCircle * 20f;
			//				p2.x = p2.x + rv.x;
			//				p2.z = p2.z + rv.y;
			//
			//				DrawLine.Add(p2, p2 + Vector3.up * 10f, "random", Color.white);
			//			}

			global.End();
			TSW.Log.Logger.Add(TSW.Profiler.Instance.ToString());
		}

		private IEnumerable<Vector3> InSurfaceDistribution(Rect surface, float step, float y)
		{
			for (float x = step + surface.xMin; x < surface.xMax; x += step)
			{
				for (float z = step + surface.yMin; z < surface.yMax; z += step)
				{
					yield return new Vector3(x, y, z);
				}
			}
		}

		private IEnumerable<Vector3> ProbePoints()
		{
			if (null == _probe)
			{
				yield break;
			}
			foreach (Vector3 vz in VectorUtils.Range(_probe.position, _probe.position + Vector3.forward * _probeSize, _levelProfile.Terrain.BlockSize))
			{
				foreach (Vector3 vx in VectorUtils.Range(vz, vz + Vector3.right * _probeSize, _levelProfile.Terrain.BlockSize))
				{
					yield return vx;
				}
			}
		}

		private void OnDrawGizmos()
		{
			foreach (Vector3 v in ProbePoints())
			{
				Gizmos.DrawLine(v, v + Vector3.up * 20f);
			}
		}

		private Rect CombineSurface(List<Rect> surfaces)
		{
			Rect comb = new Rect
			{
				xMin = float.MaxValue,
				yMin = float.MaxValue,
				xMax = float.MinValue,
				yMax = float.MinValue
			};
			foreach (Rect surf in surfaces)
			{
				comb.xMin = Mathf.Min(surf.xMin, comb.xMin);
				comb.xMax = Mathf.Max(surf.xMax, comb.xMax);
				comb.yMin = Mathf.Min(surf.yMin, comb.yMin);
				comb.yMax = Mathf.Max(surf.yMax, comb.yMax);
			}
			return comb;
		}

		public void Clear()
		{
			GameObject level = GameObject.Find("Level");
			if (level != null)
			{
				level.Destroy();
			}
		}
	}
}
