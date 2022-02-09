using UnityEngine;

namespace TSW.Gesture
{
	public enum GestureType
	{
		Tap,
		Swipe,
		Press
	}

	public class Gesture
	{
		protected Vector2 _beginPosition;
		protected float _beginTime;
		protected Vector2 _lastPosition;
		protected float _lastTime;
		protected GestureType _type;
		protected bool _finished = false;
		protected bool _hasMoved = false;

		public GestureType Type => _type;

		public Vector2 Direction => MoveVector.normalized;

		public float Duration => _lastTime - _beginTime;

		public float Strength => MoveVector.magnitude;

		protected Vector2 MoveVector => _lastPosition - _beginPosition;

		public Vector2 LastPosition => _lastPosition;

		public bool Finished => _finished;

		public bool HasMoved => _hasMoved;

		public void OnTouchBegan(TouchCopy touch)
		{
			_beginPosition = touch.position;
			_beginTime = Time.time;
			_finished = false;
			_hasMoved = false;
			Update(touch);
			SetType(GestureType.Press);
		}

		public void OnTouchMoved(TouchCopy touch)
		{
			_hasMoved = true;
			Update(touch);
		}

		public void OnTouchStationary(TouchCopy touch)
		{
			Update(touch);
		}

		public void OnTouchEnded(TouchCopy touch)
		{
			Update(touch);
			if (MoveVector.magnitude < 10)
			{
				SetType(GestureType.Tap);
			}
			else
			{
				SetType(GestureType.Swipe);
			}
			_finished = true;
		}

		public void OnTouchCanceled(TouchCopy touch)
		{
			Update(touch);
			_finished = true;
		}

		public bool IsDirection(Vector2 direction)
		{
			return Mathf.Abs(Vector2.Angle(direction, Direction)) < 10.0f;
		}

		public void Update(TouchCopy touch)
		{
			_lastPosition = touch.position;
			_lastTime = Time.time;
		}

		private void SetType(GestureType type)
		{
			_type = type;
		}
	}
}