using UnityEditor;

using UnityEngine;

namespace LevelGen
{
	[CustomPropertyDrawer(typeof(VertexModifier))]
	public class VertexModifierPropertyDrawer : PropertyDrawer
	{
		private bool _folded = true;
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			_folded = EditorGUI.PropertyField(position, property, true);
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (!_folded)
			{
				return EditorGUI.GetPropertyHeight(property, label, true);
			}
			return base.GetPropertyHeight(property, label);
		}
	}
}
