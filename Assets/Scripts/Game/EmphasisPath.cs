using System.Collections.Generic;

using TSW.Design;

using LevelGen;

using UnityEngine;

namespace Game
{
	public class EmphasisPath : USingleton<EmphasisPath>
	{
		private Transform _target;
		private Level _level;
		private int _closestPathIndex = 0;
		private float _minSqDistance = Mathf.Infinity;
		private PathLoader _pathLoader;

		[SerializeField]
		private int _checkMinDistance = 5;

		[SerializeField]
		private int _checkMaxDistance = 15;

		[SerializeField]
		private LayerMask _raycastLayer;

		[SerializeField]
		private GameObject _indicatorPrefab;
		private List<GameObject> _indicators;

		public void StartEmphasis(Transform target, LevelBuilder builder)
		{
			if (builder.IsStatic)
			{
				enabled = false;
				return;
			}
			_target = target;
			_level = builder.Level;
			_closestPathIndex = 0;
			_minSqDistance = Mathf.Infinity;
			if (_pathLoader != null)
			{
				GameObject.Destroy(_pathLoader.gameObject);
			}
			_pathLoader = PathLoader.LoadLevel(_level, name);
			enabled = true;
		}

		public void StopEmphasis()
		{
			enabled = false;
			if (_pathLoader != null)
			{
				GameObject.Destroy(_pathLoader.gameObject);
				_pathLoader = null;
			}
		}

		private void Update()
		{
			if (UnityEngine.Camera.main == null)
			{
				return;
			}
			if (null == _target)
			{
				enabled = false;
				return;
			}
			UpdateClosestPathIndex();
			ResetIndicators();
			CheckActivateIndicators();
		}

		private void CheckActivateIndicators()
		{
			//Debug.DrawLine(_path[_closestPathIndex].position, UnityEngine.Camera.main.transform.position, Color.magenta);
			List<int> toActivate = new List<int>();
			bool inRow = false;
			for (int i = Mathf.Min(_closestPathIndex + _checkMinDistance, _pathLoader.Path.Count);
				 i < Mathf.Min(_closestPathIndex + _checkMaxDistance, _pathLoader.Path.Count);
				 ++i)
			{
				Vector3 indexPos = _pathLoader.Path[i].position;
				Vector3 cp = UnityEngine.Camera.main.transform.position;
				RaycastHit hit;
				if (Physics.Raycast(cp, (indexPos + Vector3.up * 5f - cp).normalized, out hit, (indexPos - cp).magnitude, _raycastLayer))
				{
					//Debug.DrawLine(cp, indexPos, Color.green);
					if (!inRow)
					{
						inRow = true;
					}
					toActivate.Add(i - _closestPathIndex);
				}
				else
				{
					//Debug.DrawLine(cp, indexPos, Color.blue);
					if (inRow)
					{
						inRow = false;
						ActivateIndicators(toActivate);
						toActivate.Clear();
					}
				}
			}
			ActivateIndicators(toActivate);
		}

		private void ActivateIndicators(List<int> toActivate)
		{
			if (toActivate.Count > 2)
			{
				foreach (int index in toActivate)
				{
					_indicators[index].SetActive(true);
					_indicators[index].transform.position = _pathLoader.Path[index + _closestPathIndex].position;
					_indicators[index].transform.rotation = _pathLoader.Path[index + _closestPathIndex].rotation;
					//Debug.DrawLine(_indicators[index].transform.position, _indicators[index].transform.position + Vector3.up * 10f, Color.red);
				}
			}
		}

		private void ResetIndicators()
		{
			if (null == _indicators)
			{
				_indicators = new List<GameObject>();
			}
			if (_indicators.Count != _checkMaxDistance)
			{
				foreach (GameObject obj in _indicators)
				{
					GameObject.Destroy(obj);
				}
				_indicators.Clear();
				for (int i = 0; i < _checkMaxDistance; ++i)
				{
					GameObject obj = GameObject.Instantiate(_indicatorPrefab);
					obj.transform.SetParent(transform);
					_indicators.Add(obj);
				}
			}
			foreach (GameObject obj in _indicators)
			{
				obj.SetActive(false);
			}
		}

		private void UpdateClosestPathIndex()
		{
			float lastSqMagn = 0f;
			// upCount is increased by one when the iteration move away from the camera
			int upCount = 0;
			_minSqDistance = (_target.position - _pathLoader.Path[_closestPathIndex].position).sqrMagnitude;
			// _closestPathIndex is the last index known to be the closest to the camera
			foreach (int next in NextIndex(_closestPathIndex, _pathLoader.Path.Count))
			{
				float sqMagn = (_target.position - _pathLoader.Path[next].position).sqrMagnitude;
				if (sqMagn > lastSqMagn && ++upCount >= 2)
				{
					break;
				}
				else
				{
					upCount = 0;
				}
				if (sqMagn < _minSqDistance)
				{
					_minSqDistance = sqMagn;
					_closestPathIndex = next;
				}
				lastSqMagn = sqMagn;
			}
		}

		/// <summary>
		/// start, start+1, start-1, start+2, start-2 ... wrap around box
		/// example start=3, box=5: 3 4 2 5 1 0
		/// </summary>
		/// <returns>The index.</returns>
		/// <param name="start">Start.</param>
		/// <param name="box">Box.</param>
		private static IEnumerable<int> NextIndex(int start, int box)
		{
			if (box == 0)
			{
				yield break;
			}
			int offset = 0;
			int count = 0;
			int flip = 1;
			while (true)
			{
				yield return TSW.Math.Mod(start + offset, box);
				count++;
				if (count == box)
				{
					yield break;
				}
				offset = offset + count * flip;
				flip = -flip;
			}
		}
	}
}
