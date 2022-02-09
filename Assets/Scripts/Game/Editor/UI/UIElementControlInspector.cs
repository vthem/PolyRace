using UnityEditor;

using UnityEngine;

namespace Game.UI
{
	[CustomEditor(typeof(UIElementControl))]
	public class UIElementControlInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			UIElementControl control = (UIElementControl)target;
			UIElement ui = control.GetComponent<UIElement>();
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Display"))
			{
				ui.Display();
			}

			if (GUILayout.Button("DisplayAll"))
			{
				ui.DisplayAll();
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Hide"))
			{
				ui.Hide();
			}
			if (GUILayout.Button("HideAll"))
			{
				ui.HideAll();
			}
			EditorGUILayout.EndHorizontal();

		}
	}
}