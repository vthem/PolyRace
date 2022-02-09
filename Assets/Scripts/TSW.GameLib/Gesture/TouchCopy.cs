using UnityEngine;

namespace TSW.Gesture
{
	public class TouchCopy
	{
		public Vector2 deltaPosition;
		public float deltaTime;
		public int fingerId;
		public TouchPhase phase;
		public Vector2 position;
		public Vector2 rawPosition;
		public int tapCount;

		public TouchCopy(Touch touch)
		{
			deltaPosition = touch.deltaPosition;
			deltaTime = touch.deltaTime;
			fingerId = touch.fingerId;
			phase = touch.phase;
			position = touch.position;
			rawPosition = touch.rawPosition;
			tapCount = touch.tapCount;
		}

		public TouchCopy(Vector2 position, TouchPhase phase)
		{
			this.position = position;
			this.phase = phase;
		}
	}
}
