using TSW.Messaging;

using LevelGen;

using UnityEngine;

using BDEvent = TSW.Messaging.Event;

namespace Game
{
	/// <summary>
	/// Object followed by the camera. It follows the path of the level.
	/// </summary>
	public class MainMenuRabbit : MonoBehaviour
	{
		public class RabbitReachedEnd : BDEvent { }

		[SerializeField]
		private float _speed = 1f;

		[SerializeField]
		private int _headingOffset = 3;

		[SerializeField]
		private int _endOffset = 20;

		public void Run(Level level)
		{
			_pathLoader = PathLoader.LoadLevel(level, name);
			transform.position = GetPoint(0);
			_endOffset = Mathf.Max(1, _endOffset);
		}

		private PathLoader _pathLoader;
		private float _currentIndex = 0f;
		private bool _endReachedEventSent = false;

		private void Update()
		{
			if (!_pathLoader)
			{
				return;
			}
			if (!_endReachedEventSent && Mathf.FloorToInt(_currentIndex + _headingOffset + _endOffset) >= _pathLoader.Path.Count)
			{
				_endReachedEventSent = true;
				Dispatcher.FireEvent(new RabbitReachedEnd());
			}
			if (Mathf.FloorToInt(_currentIndex + _headingOffset + 1) >= _pathLoader.Path.Count)
			{
				return;
			}
			transform.position = GetPoint(_currentIndex + 1f);
			transform.forward = (GetPoint(_currentIndex + _headingOffset) - transform.position).normalized;
			_currentIndex += _speed * Time.deltaTime;
		}

		private Vector3 GetPoint(float offset)
		{
			int next = Mathf.FloorToInt(offset + 1f);
			int cur = Mathf.FloorToInt(offset);
			float n = offset % 1f;
			return Vector3.Lerp(_pathLoader.Path[cur].position, _pathLoader.Path[next].position, n);
		}

		private void Awake()
		{
			transform.position = Config.GetAsset().StartPosition;
		}

		private void OnDestroy()
		{
			if (_pathLoader != null)
			{
				GameObject.Destroy(_pathLoader.gameObject);
			}
		}
	}
}