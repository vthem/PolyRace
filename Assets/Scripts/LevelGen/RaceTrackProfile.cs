using UnityEngine;

namespace LevelGen
{
	public sealed class RaceTrackProfile : ScriptableObject
	{
		public GameObject _arrowPrefab;
		public GameObject _startArchPrefab;
		public GameObject _endArchPrefab;
		public GameObject _bonusSlotPrefab;
		public int _minArrowDistance = 20;
		public float _bezierTangentLength = 30f;
		public float _weightReduceTolerance = 100f;

		public float _averageSpeed = 800f;
		public float _startBonusTimer = 10f;
		public float _bonusTimer = 4f;

		public float _arrowOverlappingSize = 20f;
		public int _arrowOverlappingSquare = 3;
	}
}
