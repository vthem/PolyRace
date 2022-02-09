using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

namespace TSW.Gesture
{
	public class GestureManager : TSW.Design.USingleton<GestureManager>
	{
		protected Dictionary<int, Gesture> _currentGesture = new Dictionary<int, Gesture>();
		protected List<IGestureHandler> _handlerList = new List<IGestureHandler>();
		protected Dictionary<GestureType, MethodInfo> _methodDictionary = new Dictionary<GestureType, MethodInfo>();

		protected Vector2 _lastMousePosition;

		public void Register(IGestureHandler handler)
		{
			if (_handlerList.Exists(item => item == handler))
			{
				return;
			}
			_handlerList.Add(handler);
		}

		protected override void SingletonCreate()
		{
			foreach (GestureType type in (GestureType[])Enum.GetValues(typeof(GestureType)))
			{
				Type t = typeof(IGestureHandler);
				MethodInfo m = t.GetMethod("On" + type.ToString());
				_methodDictionary[type] = m;
			}
		}

		private void LateUpdate()
		{
			TouchCopy[] touches;
			if (Application.platform != RuntimePlatform.OSXEditor)
			{
				touches = new TouchCopy[Input.touches.Length];
				for (int i = 0; i < Input.touches.Length; ++i)
				{
					touches[i] = new TouchCopy(Input.touches[i]);
				}
			}
			else
			{
				if (Input.GetMouseButtonDown(0))
				{
					touches = new TouchCopy[1];
					touches[0] = new TouchCopy(Input.mousePosition, TouchPhase.Began);
				}
				else if (Input.GetMouseButtonUp(0))
				{
					touches = new TouchCopy[1];
					touches[0] = new TouchCopy(Input.mousePosition, TouchPhase.Ended);
				}
				else if (Input.GetMouseButton(0))
				{
					touches = new TouchCopy[1];
					Vector2 curPos = Input.mousePosition;
					if (_lastMousePosition != curPos)
					{
						touches[0] = new TouchCopy(curPos, TouchPhase.Moved);
					}
					else
					{
						touches[0] = new TouchCopy(curPos, TouchPhase.Stationary);
					}
				}
				else
				{
					touches = new TouchCopy[0];
				}
				_lastMousePosition = Input.mousePosition;
			}
			foreach (TouchCopy touch in touches)
			{
				switch (touch.phase)
				{
					case TouchPhase.Began:
						Began(touch);
						break;
					case TouchPhase.Moved:
						Moved(touch);
						break;
					case TouchPhase.Stationary:
						Stationary(touch);
						break;
					case TouchPhase.Canceled:
						Canceled(touch);
						break;
					case TouchPhase.Ended:
						Ended(touch);
						break;
				}
			}

			foreach (IGestureHandler handler in _handlerList)
			{
				handler.Reset();
			}
			List<int> toRemove = new List<int>();
			foreach (KeyValuePair<int, Gesture> pair in _currentGesture)
			{
				MethodInfo method;
				if (!_methodDictionary.TryGetValue(pair.Value.Type, out method))
				{
					throw new System.Exception("No method found!");
				}
				foreach (IGestureHandler handler in _handlerList)
				{
					method.Invoke(handler, new object[] { pair.Value });
				}
				if (pair.Value.Finished)
				{
					toRemove.Add(pair.Key);
				}
			}
			foreach (int fingerId in toRemove)
			{
				_currentGesture.Remove(fingerId);
			}
		}

		private void Began(TouchCopy touch)
		{
			// check if the gesture already exist or not
			Gesture gesture;
			if (!_currentGesture.TryGetValue(touch.fingerId, out gesture))
			{
				gesture = new Gesture();
				// if it does not exist, add it
				_currentGesture.Add(touch.fingerId, gesture);
			}
			gesture.OnTouchBegan(touch);
		}

		private void Moved(TouchCopy touch)
		{
			Gesture gesture;
			if (!_currentGesture.TryGetValue(touch.fingerId, out gesture))
			{
				throw new System.Exception("No gesture to handle moved id:" + touch.fingerId);
			}
			gesture.OnTouchMoved(touch);
		}

		private void Stationary(TouchCopy touch)
		{
			Gesture gesture;
			if (!_currentGesture.TryGetValue(touch.fingerId, out gesture))
			{
				throw new System.Exception("No gesture to handle stationary id:" + touch.fingerId);
			}
			gesture.OnTouchStationary(touch);
		}

		private void Canceled(TouchCopy touch)
		{
			Gesture gesture;
			if (!_currentGesture.TryGetValue(touch.fingerId, out gesture))
			{
				throw new System.Exception("No gesture to handle canceled id:" + touch.fingerId);
			}
			gesture.OnTouchCanceled(touch);
		}

		private void Ended(TouchCopy touch)
		{
			Gesture gesture;
			if (!_currentGesture.TryGetValue(touch.fingerId, out gesture))
			{
				throw new System.Exception("No gesture to handle ended id:" + touch.fingerId);
			}
			gesture.OnTouchEnded(touch);
		}
	}
}

