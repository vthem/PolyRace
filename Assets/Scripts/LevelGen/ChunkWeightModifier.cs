using System.Collections.Generic;

using TSW.Struct;
using TSW.Unity;

using UnityEngine;

namespace LevelGen
{
	public class ChunkWeightModifier
	{
		public static void ModifyWeightByEdgeDistance(float[] heights, float[] weights, Chunk chunk)
		{
			int nBlockXZ = chunk.Terrain.NumberOfBlockXZ;
			int nScalarXZ = nBlockXZ + 1;
			int nBlockY = chunk.Terrain.NumberOfBlockY;

			// find the edges
			List<Int2> workSet = new List<Int2>();
			List<Int2> nextWorkSet = new List<Int2>();
			int count = 0;
			for (int z = 0; z < nBlockXZ; ++z)
			{
				for (int x = 0; x < nBlockXZ; ++x)
				{
					count = 0;

					if (heights[z * nScalarXZ + x] * nBlockY > 1f)
					{
						count++;
					}

					if (heights[z * nScalarXZ + x + 1] * nBlockY > 1f)
					{
						count++;
					}

					if (heights[(z + 1) * nScalarXZ + x + 1] * nBlockY > 1f)
					{
						count++;
					}

					if (heights[(z + 1) * nScalarXZ + x] * nBlockY > 1f)
					{
						count++;
					}

					if (count >= 1 && count <= 3)
					{
						weights[z * nBlockXZ + x] = 1f;
						nextWorkSet.Add(new Int2(x, z));
					}
					else if (count == 4)
					{
						weights[z * nBlockXZ + x] = 1f;
					}
					else
					{
						weights[z * nBlockXZ + x] = -1f;
					}
				}
			}

			// all block in nextWorkSet are edges
			// from the edge, find neighbors, assign weights
			float weight = 0f;
			float neighborWeight = 0f;
			workSet = nextWorkSet;
			nextWorkSet = new List<Int2>();
			while (workSet.Count > 0)
			{
				foreach (Int2 pos in workSet)
				{
					weight = weights[pos.z * nBlockXZ + pos.x] + 1f;
					foreach (Int2 neighbor in Int2.Neighbors(pos))
					{
						if (neighbor.x < 0 || neighbor.z < 0 || neighbor.x >= nBlockXZ || neighbor.z >= nBlockXZ)
						{
							continue;
						}
						neighborWeight = weights[neighbor.z * nBlockXZ + neighbor.x];
						if (neighborWeight < 0f)
						{ // never set
							nextWorkSet.Add(neighbor);
							weights[neighbor.z * nBlockXZ + neighbor.x] = weight;
						}
						else if (neighborWeight > weight)
						{ // set, closer to this edge
							weights[neighbor.z * nBlockXZ + neighbor.x] = weight;
						}
					}
				}
				workSet = nextWorkSet;
				nextWorkSet = new List<Int2>();
			}

			//			DrawWeight(weights, chunk, "raw-weights", Color.magenta);

			// reverse weights
			float blockSize = chunk.Terrain.BlockSize;
			for (int z = 0; z < nBlockXZ; ++z)
			{
				for (int x = 0; x < nBlockXZ; ++x)
				{
					if (chunk.IsBorderOffset(x, z, 2))
					{
						weights[z * nBlockXZ + x] = Chunk.WeightLimit;
					}
					else
					{
						weights[z * nBlockXZ + x] = chunk.Terrain.WeightModifier.Evaluate((weights[z * nBlockXZ + x] - 1f) * blockSize);
					}
				}

			}
		}

		private static void DrawWeight(float[] weights, Chunk chunk, string tag, Color color)
		{
			int nBlockXZ = chunk.Terrain.NumberOfBlockXZ;
			Int2 cur;
			for (int z = 0; z < nBlockXZ; ++z)
			{
				for (int x = 0; x < nBlockXZ; ++x)
				{
					cur.x = x; cur.z = z;
					cur = chunk.WorldBlockPosition(cur);
					DrawLine.Add(chunk.Level.BlockToWorld(cur) + Vector3.up * 100f, chunk.Level.BlockToWorld(cur) + (Vector3.right + Vector3.forward) * weights[z * nBlockXZ + x] * chunk.Terrain.BlockSize / (chunk.Terrain.NumberOfBlockXZ / 2f) + Vector3.up * 100f, tag, color);
				}
			}
		}
	}
}
