using UnityEngine;

namespace TSW.Unity
{
	public class InGameScreenshot : MonoBehaviour
	{
		public KeyCode _keyCode;
		public string _prefix;
		public int _factor = 1;

		// Update is called once per frame
		private void Update()
		{
			if (Input.GetKeyDown(_keyCode))
			{
				System.DateTime today = System.DateTime.Now;
				string name = _prefix + "-" + today.DayOfYear + "_" + today.Hour + "_" + today.Minute + "_" + today.Second + "_" + Random.Range(0, int.MaxValue) + "-x" + _factor + ".png";
				ScreenCapture.CaptureScreenshot(name, _factor);
			}
		}
	}
}