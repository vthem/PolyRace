using UnityEditor;

using UnityEngine;

namespace LevelGen
{
	[CustomEditor(typeof(WorldChunkBehaviour))]
	public class WorldChunkBehaviourEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			WorldChunkBehaviour behaviour = target as WorldChunkBehaviour;
			if (GUILayout.Button("Create"))
			{
				behaviour.Create();
			}
			if (GUILayout.Button("Clear"))
			{
				behaviour.Clear();
			}
		}
	}
}
