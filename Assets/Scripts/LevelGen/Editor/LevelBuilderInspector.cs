using UnityEditor;

using UnityEngine;

namespace LevelGen
{
	[CustomEditor(typeof(LevelBuilder))]
	public class LevelBuilderInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			LevelBuilder builder = target as LevelBuilder;
			if (GUILayout.Button("Create"))
			{
				builder.CreateStatic();
			}
			if (GUILayout.Button("Clear"))
			{
				builder.Clear();
			}
			DrawDefaultInspector();
		}
	}
}