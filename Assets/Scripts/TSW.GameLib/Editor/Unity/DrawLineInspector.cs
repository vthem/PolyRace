using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

namespace TSW.Unity
{
	[CustomEditor(typeof(DrawLine))]
	public class LevelScriptEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			if (GUILayout.Button("Clear all"))
			{
				DrawLine.ClearAll();
				SceneView.RepaintAll();
			}
			List<string> tags = DrawLine.GetAllTags();
			foreach (string tag in tags)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(tag + ": ");
				if (DrawLine.IsTagActive(tag))
				{
					if (GUILayout.Button("disable"))
					{
						DrawLine.SetStateByTag(tag, false);
						SceneView.RepaintAll();
					}
				}
				else
				{
					if (GUILayout.Button("enable"))
					{
						DrawLine.SetStateByTag(tag, true);
						SceneView.RepaintAll();
					}
				}
				if (GUILayout.Button("clear"))
				{
					DrawLine.Clear(tag);
					SceneView.RepaintAll();
				}
				EditorGUILayout.EndHorizontal();
			}
		}
	}
}
