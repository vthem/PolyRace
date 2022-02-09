using UnityEngine;

// from : http://wiki.unity3d.com/index.php?title=FramesPerSecond

namespace TSW
{
	public class FramPerSecond
	{

		// Attach this to a GUIText to make a frames/second indicator.
		//
		// It calculates frames/second over each updateInterval,
		// so the display does not keep changing wildly.
		//
		// It is also fairly accurate at very low FPS counts (<10).
		// We do this not by simply counting frames per interval, but
		// by accumulating FPS for each frame. This way we end up with
		// correct overall FPS even if the interval renders something like
		// 5.5 frames.

		private readonly float _updateInterval = 0.5F;
		private float _accum = 0; // FPS accumulated over the interval
		private int _frames = 0; // Frames drawn over the interval
		private float _timeleft; // Left time for current interval
		public float FPS { get; private set; }

		public FramPerSecond(float updateInterval)
		{
			_updateInterval = updateInterval;
			_timeleft = _updateInterval;
		}

		public void Update()
		{
			_timeleft -= Time.deltaTime;
			_accum += Time.timeScale / Time.deltaTime;
			++_frames;

			// Interval ended - update GUI text and start new interval
			if (_timeleft <= 0.0)
			{
				// display two fractional digits (f2 format)
				FPS = _accum / _frames;

				_timeleft = _updateInterval;
				_accum = 0.0F;
				_frames = 0;
			}
		}
	}
}