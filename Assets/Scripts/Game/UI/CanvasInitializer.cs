using System.Collections.Generic;

using UnityEngine;

namespace Game.UI
{
	public class CanvasInitializer : MonoBehaviour
	{
		private void Awake()
		{
			List<UIElement> l = new List<UIElement>();
			FindChildUIElement(transform, l);
			foreach (UIElement ui in l)
			{
				ui.Initialize();
				ui.UpdateDisplayState(ui.DisplayAwake, true);
			}
		}

		private static void FindChildUIElement(Transform t, List<UIElement> l)
		{
			UIElement ui = t.GetComponent<UIElement>();
			if (ui != null)
			{
				l.Add(ui);
			}
			foreach (Transform child in t)
			{
				FindChildUIElement(child, l);
			}
		}
	}
}
