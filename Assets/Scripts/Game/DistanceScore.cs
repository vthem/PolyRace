using System.Collections.Generic;
using System.Text;

using LevelGen;

using UnityEngine;

namespace Game
{
	// No SINGLETON! WHAT IF WE HAVE MULTIPLE PLAYER? WE NEED MULTIPLE SCORE AT THE SAME TIME
	public class DistanceScore : MonoBehaviour
	{
		public float Score { get; private set; }
		public string ScoreString
		{
			get
			{
				_scoreStringBuilder.Length = 0;
				_scoreStringBuilder.AppendFormat("{0}m", Mathf.RoundToInt(Score));
				return _scoreStringBuilder.ToString();
			}
		}

		public float Distance { get; private set; }

		private readonly StringBuilder _scoreStringBuilder = new StringBuilder();
		private const string _prefabPath = "Prefabs/Game/DistanceScore";
		private Racer.Controller _racerController;
		private Level _level;
		private Vector3 _lastPosition;
		private int _closestPathIndex = 0;
		private float _minSqDistance = Mathf.Infinity;
		private PathLoader _pathLoader;

		public static DistanceScore Instantiate()
		{
			GameObject obj = Instantiate(Resources.Load(_prefabPath)) as GameObject;
			DistanceScore es = obj.GetComponent<DistanceScore>();
			es.enabled = false;
			return es;
		}

		public void StartScoring(Racer.Controller racerController, LevelBuilder builder)
		{
			Score = 0f;
			Distance = 0f;
			_lastPosition = racerController.transform.position;
			_racerController = racerController;
			TSW.OnDestroyNotifier.RegisterOnDestroy(_racerController, OnDestroyRacerController);

			_level = builder.Level;
			DeletePathLoader();
			_pathLoader = PathLoader.LoadLevel(_level, name);
			_closestPathIndex = 0;

			enabled = true;
		}

		public void OnDestroyRacerController(Transform target)
		{
			enabled = false;
			_racerController = null;
			DeletePathLoader();
		}

		private void OnDestroy()
		{
			TSW.OnDestroyNotifier.UnregisterOnDestroy(_racerController, OnDestroyRacerController);
			DeletePathLoader();
		}

		private void DeletePathLoader()
		{
			if (_pathLoader != null)
			{
				GameObject.Destroy(_pathLoader.gameObject);
			}
		}

		public void PauseScoring()
		{
			enabled = false;
		}

		public void ResumeScoring()
		{
			if (_racerController != null)
			{
				enabled = true;
			}
		}

		public void StopScoring()
		{
			enabled = false;
			_racerController = null;
			DeletePathLoader();
		}

		private void UpdateClosestPathIndex()
		{
			float lastSqMagn = 0f;
			// upCount is increased by one when the iteration move away from the racer
			int upCount = 0;
			_minSqDistance = (_racerController.transform.position - _pathLoader.Path[_closestPathIndex].position).sqrMagnitude;
			// _closestPathIndex is the last index known to be the closest to the racer
			foreach (int next in NextIndex(_closestPathIndex, _pathLoader.Path.Count))
			{
				float sqMagn = (_racerController.transform.position - _pathLoader.Path[next].position).sqrMagnitude;
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

		private void FixedUpdate()
		{
			Distance += (_racerController.transform.position - _lastPosition).magnitude;
			_lastPosition = _racerController.transform.position;

		}

		private void Update()
		{
			if (null == _racerController)
			{
				enabled = false;
				return;
			}
			UpdateClosestPathIndex();
			Score = _closestPathIndex * _level.Profile._raceTrack._minArrowDistance;
		}
	}
}
