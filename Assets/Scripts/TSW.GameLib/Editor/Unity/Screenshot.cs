using UnityEditor;

using UnityEngine;

namespace TSW.Unity
{
	public static class Screenshot
	{
		[MenuItem("TSW/Take screenshot x4")]
		private static void TakeScreenshotX4()
		{
			TakeScreenshot(4);
		}

		[MenuItem("TSW/Take screenshot x2")]
		private static void TakeScreenshotX2()
		{
			TakeScreenshot(2);
		}

		[MenuItem("TSW/Take screenshot x1")]
		private static void TakeScreenshotX1()
		{
			TakeScreenshot(1);
		}

		private static void TakeScreenshot(int factor)
		{
			System.DateTime today = System.DateTime.Now;
			string name = "Editor_" + today.DayOfYear + "_" + today.Hour + "_" + today.Minute + "_" + today.Second + "_" + Random.Range(0, int.MaxValue) + "-x" + factor + ".png";
			ScreenCapture.CaptureScreenshot(name, factor);
			Debug.Log("Screenshot save as " + name);
		}
	}
}
