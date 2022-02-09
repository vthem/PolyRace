using UnityEditor;

using UnityEngine;

namespace Game.Racer
{
	[CustomEditor(typeof(Properties))]
	public class PropertiesInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			Properties cprop = (Properties)target;
			DynamicProperties dynProp = new DynamicProperties(cprop, new DynamicPropertyPoints());
			DynamicPropertyPoints maxPoints = new DynamicPropertyPoints();
			for (int i = 0; i < maxPoints.values.Length; ++i)
			{
				maxPoints.values[i] = DynamicPropertyPoints.max;
			}
			DynamicProperties maxDynProp = new DynamicProperties(cprop, maxPoints);
			GUILayout.BeginVertical();
			string[] characteristicsIds = new string[] { "Speed", "BoostSpeed", "TurnSpeed", "TrajectoryControl" };
			foreach (string id in characteristicsIds)
			{
				AutoLayoutValue(id, Game.UI.RacerPropertyHelper.GetPropertyDisplayValue(id, dynProp), Game.UI.RacerPropertyHelper.GetPropertyDisplayValue(id, maxDynProp));
			}

			AutoLayoutValue("Battery Capacity", dynProp.ShieldCapacity.ToString("0.00"));
			AutoLayoutValue("Shield Cost", dynProp.ShieldCost.ToString("0.00"));
			AutoLayoutValue("Shield Delay", dynProp.ShieldDelay.ToString("0.00"));
			AutoLayoutValue("Drag", dynProp.Drag.ToString("0.0000"));
			AutoLayoutValue("Boost Drag", dynProp.BoostDrag.ToString("0.0000"));

			GUILayout.EndVertical();
		}

		private void AutoLayoutValue(string name, string value, string valueMax)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(name);
			GUILayout.Label(value, GUILayout.ExpandWidth(false), GUILayout.MinWidth(80f));
			GUILayout.Label(valueMax, GUILayout.ExpandWidth(false), GUILayout.MinWidth(80f));
			GUILayout.EndHorizontal();
		}

		private void AutoLayoutValue(string name, string value)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(name);
			GUILayout.Label(value, GUILayout.ExpandWidth(false), GUILayout.MinWidth(80f));
			GUILayout.EndHorizontal();
		}

	}
}
